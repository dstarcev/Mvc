// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNet.FileSystems;
using Microsoft.AspNet.Razor;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Microsoft.AspNet.Razor.Parser.SyntaxTree;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor
{
    public class RazorCompilationTest
    {
        [Fact]
        public void Compile_ReturnsFailedResultIfParseFails()
        {
            // Arrange
            var generatorResult = new GeneratorResults(
                    new Block(
                        new BlockBuilder { Type = BlockType.Comment }),
                        new RazorError[] { new RazorError("some message", 1, 1, 1, 1) },
                        new CodeBuilderResult("", new LineMapping[0]),
                        new CodeTree());
            var host = new Mock<IMvcRazorHost>();
            host.Setup(h => h.GenerateCode(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(generatorResult)
                .Verifiable();

            var fileInfo = new Mock<IFileInfo>();
            fileInfo.Setup(f => f.CreateReadStream())
                    .Returns(Stream.Null);

            var compiler = new Mock<ICompilationService>(MockBehavior.Strict);
            var relativeFileInfo = new RelativeFileInfo(fileInfo.Object, @"views\index\home.cshtml");

            // Act
            var result = RazorCompilation.Compile(host.Object, compiler.Object, relativeFileInfo);

            // Assert
            var ex = Assert.Throws<CompilationFailedException>(() => result.CompiledType);
            Assert.Equal("some message", Assert.Single(ex.Messages).Message);
            host.Verify();
        }

        [Fact]
        public void Compile_ReturnsResultFromCompilationServiceIfParseSucceeds()
        {
            // Arrange
            var code = "compiled-content";
            var prefix = "main-class-prefix";
            var generatorResult = new GeneratorResults(
                    new Block(
                        new BlockBuilder { Type = BlockType.Comment }),
                        new RazorError[0],
                        new CodeBuilderResult(code, new LineMapping[0]),
                        new CodeTree());
            var host = new Mock<IMvcRazorHost>();
            host.Setup(h => h.GenerateCode(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(generatorResult);
            host.SetupGet(h => h.MainClassNamePrefix)
                .Returns(prefix);

            var fileInfo = new Mock<IFileInfo>();
            fileInfo.Setup(f => f.CreateReadStream())
                    .Returns(Stream.Null);

            var compilationResult = CompilationResult.Successful(typeof(object));
            var compiler = new Mock<ICompilationService>();
            compiler.Setup(c => c.Compile(fileInfo.Object, code, prefix))
                    .Returns(compilationResult)
                    .Verifiable();
            var relativeFileInfo = new RelativeFileInfo(fileInfo.Object, @"views\index\home.cshtml");

            // Act
            var result = RazorCompilation.Compile(host.Object, compiler.Object, relativeFileInfo);

            // Assert
            Assert.Same(compilationResult, result);
            compiler.Verify();
        }
    }
}
