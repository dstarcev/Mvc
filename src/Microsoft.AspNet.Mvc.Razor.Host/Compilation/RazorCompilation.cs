// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.Razor;

namespace Microsoft.AspNet.Mvc.Razor
{
    /// <summary>
    /// Utility type for parsing and compiling Razor files.
    /// </summary>
    public static class RazorCompilation
    {
        /// <summary>
        /// Parses the contents of the file specified by <paramref name="file"/> and returns the results of
        /// its compilation.
        /// </summary>
        /// <param name="razorHost">The <see cref="IMvcRazorHost"/> used to parse Razor files.</param>
        /// <param name="compilationService">The <see cref="ICompilationService"/> used to compile the parsed result.
        /// </param>
        /// <param name="file">The file to compile.</param>
        /// <returns>A <see cref="CompilationResult"/> that represents the result of compilation.</returns>
        public static CompilationResult Compile([NotNull] IMvcRazorHost razorHost,
                                                [NotNull] ICompilationService compilationService,
                                                [NotNull] RelativeFileInfo file)
        {
            GeneratorResults generatorResult;
            using (var inputStream = file.FileInfo.CreateReadStream())
            {
                generatorResult = razorHost.GenerateCode(
                    file.RelativePath, inputStream);
            }

            if (!generatorResult.Success)
            {
                var messages = generatorResult.ParserErrors.Select(e => new CompilationMessage(e.Message));
                return CompilationResult.Failed(file.FileInfo, generatorResult.GeneratedCode, messages);
            }

            return compilationService.Compile(file.FileInfo, generatorResult.GeneratedCode, razorHost.MainClassNamePrefix);
        }
    }
}
