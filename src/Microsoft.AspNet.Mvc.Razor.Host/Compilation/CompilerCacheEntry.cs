// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// An entry in <see cref="ICompilerCache"/> that contain metadata about precompiled and dynamically compiled file.
    /// </summary>
    public class CompilerCacheEntry
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CompilerCacheEntry"/> for a file that was precompiled.
        /// </summary>
        /// <param name="info">Metadata about the precompiled file.</param>
        /// <param name="compiledType">The compiled <see cref="Type"/>.</param>
        public CompilerCacheEntry([NotNull] RazorFileInfo info, [NotNull] Type compiledType)
        {
            CompiledType = compiledType;
            RelativePath = info.RelativePath;
            Length = info.Length;
            LastModified = info.LastModified;
            Hash = info.Hash;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="CompilerCacheEntry"/> for a file that was dynamically compiled.
        /// </summary>
        /// <param name="info">Metadata about the file that was compiled.</param>
        /// <param name="compiledType">The compiled <see cref="Type"/>.</param>
        public CompilerCacheEntry([NotNull] RelativeFileInfo info, [NotNull] Type compiledType)
        {
            CompiledType = compiledType;
            RelativePath = info.RelativePath;
            Length = info.FileInfo.Length;
            LastModified = info.FileInfo.LastModified;
            CompiledTime = DateTime.Now;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> produced as a result of compilation.
        /// </summary>
        public Type CompiledType { get; }

        /// <summary>
        /// Gets the path of the compiled file relative to the root of the application.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// Gets the size of file (in bytes) on disk.
        /// </summary>
        public long Length { get; }

        /// <summary>
        /// Gets or sets the last modified <see cref="DateTime"/> for the file at the time of compilation.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the file was compiled.
        /// This can be different from <see cref="LastModified"/> when a file was recompiled due to changes in
        /// associated _ViewStart files.
        /// </summary>
        /// <remarks>
        /// A value for this is only available for views compiled during runtime.
        /// </remarks>
        public DateTime CompiledTime { get; }

        /// <summary>
        /// Gets the file hash, should only be available for pre compiled files.
        /// </summary>
        public string Hash { get; }

        /// <summary>
        /// Gets a flag that indicates if the file is precompiled.
        /// </summary>
        public bool IsPreCompiled { get { return Hash != null; } }

        /// <summary>
        /// Gets a <see cref="IDictionary{object, object}"/> that contains metadata associated with
        /// this instance of <see cref="CompilerCacheEntry"/>.
        /// </summary>
        /// <remarks>
        /// This collection is not thread safe and must be synchronized externally.
        /// </remarks>
        public IDictionary<object, object> Metadata { get; } = new Dictionary<object, object>();

        /// <summary>
        /// Gets or sets the <see cref="CompilerCacheEntry"/> for the nearest ViewStart that the compiled type
        /// depends on.
        /// </summary>
        public CompilerCacheEntry AssociatedViewStartEntry { get; set; }
    }
}
