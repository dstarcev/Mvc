// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.FileSystems;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// A stub implementation of <see cref="ICompilationService"/>.
    /// </summary>
    public class NullCompilationService : ICompilationService
    {
        /// <inheritdoc />
        public CompilationResult Compile(IFileInfo fileInfo, string compilationContent, string mainClassPrefix)
        {
            return CompilationResult.Successful(typeof(void));
        }
    }
}