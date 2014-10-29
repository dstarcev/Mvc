// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// Caches the result of runtime compilation of Razor files for the duration of the app lifetime.
    /// </summary>
    public interface ICompilerCache
    {
        /// <summary>
        /// Get an existing compilation result, or create and add a new one if it is
        /// not available in the cache or is invalid.
        /// </summary>
        /// <param name="fileInfo">A <see cref="RelativeFileInfo"/> representing the file.</param>
        /// <param name="compile">An delegate that will generate a compilation result.</param>
        /// <returns>A cached <see cref="CompilationResult"/>.</returns>
        CompilationResult GetOrAdd([NotNull] RelativeFileInfo fileInfo,
                                   [NotNull] Func<RelativeFileInfo, CompilationResult> compile);

        /// <summary>
        /// Gets existing metadata associated with the <see cref="CompilationResult"/> for the entry specified by
        /// <paramref name="fileInfo"/>, or creates and adds a new one if the entry is not available or has
        /// expired.
        /// </summary>
        /// <param name="fileInfo">A <see cref="RelativeFileInfo"/> representing the file.</param>
        /// <param name="key">The key for the metadata.</param>
        /// <param name="valueFactory">The factory used to create new metadata values.</param>
        /// <returns>The metadata value.</returns>
        object GetOrAddMetadata(RelativeFileInfo fileInfo,
                                Func<RelativeFileInfo, CompilationResult> compile,
                                object key,
                                Func<object> valueFactory);
    }
}