// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <inheritdoc />
    public class CompilerCache : ICompilerCache
    {
        private readonly IFileSystem _fileSystem;
        private readonly ConcurrentDictionary<string, CompilerCacheEntry> _cache =
            new ConcurrentDictionary<string, CompilerCacheEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes a new instance of <see cref="CompilerCache"/> that is populated with precompiled views
        /// discovered using <paramref name="assemblyProvider"/>.
        /// </summary>
        /// <param name="assemblyProvider">The <see cref="IAssemblyProvider"/> used to discover precompiled views.
        /// </param>
        /// <param name="fileSystem">An <see cref="ICachedFileSystem"/> that represents application's file system.
        /// </param>
        public CompilerCache(IAssemblyProvider assemblyProvider, ICachedFileSystem fileSystem)
            : this (GetFileInfos(assemblyProvider), fileSystem)
        {
        }

        // Internal for unit testing
        internal CompilerCache(IEnumerable<RazorFileInfoCollection> razorFileInfos, ICachedFileSystem fileSystem)
            : this(fileSystem)
        {
            _fileSystem = fileSystem;
            foreach (var viewCollection in razorFileInfos)
            {
                var containingAssembly = viewCollection.GetType().GetTypeInfo().Assembly;

                foreach (var fileInfo in viewCollection.FileInfos)
                {
                    var viewType = containingAssembly.GetType(fileInfo.FullTypeName);

                    // There shouldn't be any duplicates and if there are any the first will win.
                    // If the result doesn't match the one on disk its going to recompile anyways.
                    var normalizedPath = NormalizePath(fileInfo.RelativePath);
                    var entry = new CompilerCacheEntry(fileInfo, viewType);
                    _cache[normalizedPath] = entry;
                }
            }

            // Set up ViewStarts
            foreach (var entry in _cache)
            {
                // where app is deployed without cshtml files.
                var viewStartLocations = ViewStartUtility.GetViewStartLocations(fileSystem, entry.Key);
                foreach (var location in viewStartLocations)
                {
                    CompilerCacheEntry viewStartEntry;
                    if (_cache.TryGetValue(location, out viewStartEntry))
                    {
                        // Add the the first \ nearest _ViewStart entry as a dependency.
                        entry.Value.AssociatedViewStartEntry = viewStartEntry;
                        break;
                    }
                }
            }
        }

        private CompilerCache(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Creates a new instance of <see cref="CompilerCache"/>.
        /// </summary>
        /// <param name="fileSystem">An <see cref="IFileSystem"/> that represents application's file system.</param>
        /// <remarks>This factory method is meant to be used during precompilation and design time when precompiled
        /// views are not available. It is designed as a factory method for depenedency injection to correctly
        /// locate the single public constructor.</remarks>
        public static CompilerCache Create(IFileSystem fileSystem)
        {
            return new CompilerCache(fileSystem);
        }

        /// <inheritdoc />
        public CompilationResult GetOrAdd([NotNull] RelativeFileInfo fileInfo,
                                          [NotNull] Func<RelativeFileInfo, CompilationResult> compile)
        {
            CompilationResult result;
            var entry = GetOrAddEntry(fileInfo, compile, out result);
            return result;
        }

        /// <inheritdoc />
        public object GetOrAddMetadata([NotNull] RelativeFileInfo fileInfo,
                                       [NotNull] Func<RelativeFileInfo, CompilationResult> compile,
                                       [NotNull] object key,
                                       [NotNull] Func<object> valueFactory)
        {
            CompilationResult result;
            var entry = GetOrAddEntry(fileInfo, compile, out result);

            lock (entry.Metadata)
            {
                object value;
                if (!entry.Metadata.TryGetValue(key, out value))
                {
                    value = valueFactory();
                    entry.Metadata[key] = value;
                }

                return value;
            }
        }

        private CompilerCacheEntry GetOrAddEntry(RelativeFileInfo relativeFileInfo,
                                                 Func<RelativeFileInfo, CompilationResult> compile,
                                                 out CompilationResult result)
        {
            CompilerCacheEntry cacheEntry;
            var normalizedPath = NormalizePath(relativeFileInfo.RelativePath);
            if (!_cache.TryGetValue(normalizedPath, out cacheEntry))
            {
                return OnCacheMiss(relativeFileInfo, normalizedPath, compile, out result);
            }
            else
            {
                var fileInfo = relativeFileInfo.FileInfo;
                if (cacheEntry.Length != fileInfo.Length)
                {
                    // Recompile if the file lengths differ
                    return OnCacheMiss(relativeFileInfo, normalizedPath, compile, out result);
                }

                if (AssociatedViewStartsChanged(cacheEntry, compile))
                {
                    // Recompile if the view starts have changed since the entry was created.
                    return OnCacheMiss(relativeFileInfo, normalizedPath, compile, out result);
                }

                if (cacheEntry.LastModified == fileInfo.LastModified)
                {
                    result = CompilationResult.Successful(cacheEntry.CompiledType);
                    return cacheEntry;
                }

                // Timestamp doesn't match but it might be because of deployment, compare the hash.
                if (cacheEntry.IsPreCompiled &&
                    string.Equals(cacheEntry.Hash, RazorFileHash.GetHash(fileInfo), StringComparison.Ordinal))
                {
                    // Cache hit, but we need to update the entry
                    cacheEntry.LastModified = fileInfo.LastModified;
                    result = CompilationResult.Successful(cacheEntry.CompiledType);

                    return cacheEntry;
                }

                // it's not a match, recompile
                return OnCacheMiss(relativeFileInfo, normalizedPath, compile, out result);
            }
        }

        private CompilerCacheEntry OnCacheMiss(RelativeFileInfo file,
                                               string normalizedPath,
                                               Func<RelativeFileInfo, CompilationResult> compile,
                                               out CompilationResult result)
        {
            result = compile(file);

            var cacheEntry = new CompilerCacheEntry(file, result.CompiledType)
            {
                AssociatedViewStartEntry = GetNearestViewStartEntry(normalizedPath, compile)
            };

            _cache[normalizedPath] = cacheEntry;
            return cacheEntry;
        }

        private bool AssociatedViewStartsChanged(CompilerCacheEntry entry,
                                                 Func<RelativeFileInfo, CompilationResult> compile)
        {
            var viewStartEntry = GetNearestViewStartEntry(entry.RelativePath, compile);
            return entry.AssociatedViewStartEntry != viewStartEntry;
        }

        private CompilerCacheEntry GetNearestViewStartEntry(string relativePath,
                                                            Func<RelativeFileInfo, CompilationResult> compile)
        {
            var viewStartLocations = ViewStartUtility.GetViewStartLocations(_fileSystem, relativePath);
            foreach (var viewStartLocation in viewStartLocations)
            {
                IFileInfo viewStartFileInfo;
                if (_fileSystem.TryGetFileInfo(viewStartLocation, out viewStartFileInfo))
                {
                    var relativeFileInfo = new RelativeFileInfo(viewStartFileInfo, viewStartLocation);
                    CompilationResult result;
                    return GetOrAddEntry(relativeFileInfo, compile, out result);
                }
            }

            return null;
        }

        internal static IEnumerable<RazorFileInfoCollection> GetFileInfos(IAssemblyProvider assemblyProvider)
        {
            return assemblyProvider
                    .CandidateAssemblies
                    .SelectMany(a => a.ExportedTypes)
                    .Where(Match)
                    .Select(c => (RazorFileInfoCollection)Activator.CreateInstance(c));
        }

        private static bool Match(Type t)
        {
            var inAssemblyType = typeof(RazorFileInfoCollection);
            if (inAssemblyType.IsAssignableFrom(t))
            {
                var hasParameterlessConstructor = t.GetConstructor(Type.EmptyTypes) != null;

                return hasParameterlessConstructor
                    && !t.GetTypeInfo().IsAbstract
                    && !t.GetTypeInfo().ContainsGenericParameters;
            }

            return false;
        }

        private static string NormalizePath(string path)
        {
            path = path.Replace('/', '\\');
            path = path.TrimStart('\\');

            return path;
        }
    }
}
