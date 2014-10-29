// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.PageExecutionInstrumentation;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// Represents a <see cref="IRazorPageFactory"/> that creates <see cref="RazorPage"/> instances
    /// from razor files in the file system.
    /// </summary>
    public class VirtualPathRazorPageFactory : IRazorPageFactory
    {
        private readonly ITypeActivator _activator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ICachedFileSystem _cachedFileSystem;
        private readonly ICompilerCache _compilerCache;
        private IMvcRazorHost _razorHost;
        private ICompilationService _compilationService;

        public VirtualPathRazorPageFactory(ITypeActivator typeActivator,
                                           IServiceProvider serviceProvider,
                                           ICompilerCache compilerCache,
                                           ICachedFileSystem cachedFileSystem)
        {
            _activator = typeActivator;
            _serviceProvider = serviceProvider;
            _compilerCache = compilerCache;
            _cachedFileSystem = cachedFileSystem;
        }

        /// <inheritdoc />
        public IRazorPage CreateInstance([NotNull] string relativePath)
        {
            if (relativePath.StartsWith("~/", StringComparison.Ordinal))
            {
                // For tilde slash paths, drop the leading ~ to make it work with the underlying IFileSystem.
                relativePath = relativePath.Substring(1);
            }

            IFileInfo fileInfo;
            if (_cachedFileSystem.TryGetFileInfo(relativePath, out fileInfo))
            {
                EnsureCompilationServices();
                var relativeFileInfo = new RelativeFileInfo(fileInfo, relativePath);

                var result = _compilerCache.GetOrAdd(
                    relativeFileInfo,
                    (f) => RazorCompilation.Compile(_razorHost, _compilationService, f));

                var page = (IRazorPage)_activator.CreateInstance(_serviceProvider, result.CompiledType);
                page.Path = relativePath;

                return page;
            }

            return null;
        }

        private void EnsureCompilationServices()
        {
            if (_compilationService == null)
            {
                // it is ok to use the cached service provider because both this, and the
                // resolved services are in a lifetime of Scoped.
                // We don't want to get it upfront because it will force Roslyn to load.
                _compilationService = _serviceProvider.GetRequiredService<ICompilationService>();
                _razorHost = _serviceProvider.GetRequiredService<IMvcRazorHost>();
            }
        }


        private static bool IsInstrumentationEnabled(HttpContext context)
        {
            return context.GetFeature<IPageExecutionListenerFeature>() != null;
        }
    }
}
