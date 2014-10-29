// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Razor.Generator.Compiler;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor.Directives
{
    public class ChunkInheritanceUtilityTest
    {
        [Fact]
        public void GetInheritedChunks_ReadsChunksFromViewStartsInPath()
        {
            // Arrange
            var fileSystem = new TestFileSystem();
            fileSystem.AddFile(@"x:\myapproot\views\accounts\_viewstart.cshtml", "@using AccountModels");
            fileSystem.AddFile(@"x:\myapproot\views\Shared\_viewstart.cshtml", "@inject SharedHelper Shared");
            fileSystem.AddFile(@"x:\myapproot\views\home\_viewstart.cshtml", "@using MyNamespace");
            fileSystem.AddFile(@"x:\myapproot\views\_viewstart.cshtml",
@"@inject MyHelper<TModel> Helper
@inherits MyBaseType

@{
    Layout = ""test.cshtml"";
}

");
            var host = new MvcRazorHost(fileSystem, new NullCompilationService(), CompilerCache.Create(fileSystem));
            var utility = host.ChunkInheritanceUtility;

            // Act
            var chunks = utility.GetInheritedChunks(@"x:\myapproot\views\home\Index.cshtml");

            // Assert
            Assert.Equal(11, chunks.Count);
            Assert.IsType<LiteralChunk>(chunks[0]);

            var usingChunk = Assert.IsType<UsingChunk>(chunks[1]);
            Assert.Equal("MyNamespace", usingChunk.Namespace);

            Assert.IsType<LiteralChunk>(chunks[2]);
            Assert.IsType<LiteralChunk>(chunks[3]);

            var injectChunk = Assert.IsType<InjectChunk>(chunks[4]);
            Assert.Equal("MyHelper<TModel>", injectChunk.TypeName);
            Assert.Equal("Helper", injectChunk.MemberName);

            var setBaseTypeChunk = Assert.IsType<SetBaseTypeChunk>(chunks[5]);
            Assert.Equal("MyBaseType", setBaseTypeChunk.TypeName);

            Assert.IsType<StatementChunk>(chunks[6]);
            Assert.IsType<LiteralChunk>(chunks[7]);
        }

        [Fact]
        public void GetInheritedChunks_ReturnsEmptySequenceIfNoViewStartsArePresent()
        {
            // Arrange
            var fileSystem = new TestFileSystem();
            fileSystem.AddFile(@"x:\myapproot\_viewstart.cs", string.Empty);
            fileSystem.AddFile(@"x:\myapproot\views\_Layout.cshtml", string.Empty);
            fileSystem.AddFile(@"x:\myapproot\views\home\_not-viewstart.cshtml", string.Empty);
            var host = new MvcRazorHost(fileSystem, new NullCompilationService(), CompilerCache.Create(fileSystem));
            var utility = host.ChunkInheritanceUtility;

            // Act
            var chunks = utility.GetInheritedChunks(@"x:\myapproot\views\home\Index.cshtml");

            // Assert
            Assert.Equal<Chunk>(host.DefaultInheritedChunks, chunks);
        }

        [Fact]
        public void GetInheritedChunks_ReturnsDefaultInheritedChunks()
        {
            // Arrange
            var fileSystem = new TestFileSystem();
            fileSystem.AddFile(@"x:\myapproot\views\_viewstart.cshtml",
                               "@inject DifferentHelper<TModel> Html");
            var host = new Mock<MvcRazorHost>(fileSystem, new NullCompilationService(), CompilerCache.Create(fileSystem))
            {
                CallBase = true
            };
            var span = SpanFactory.CreateCsHtml().Code("using").AsStatement();

            var defaultChunks = new Chunk[]
            {
                new InjectChunk("MyTestHtmlHelper", "Html"),
                new UsingChunk { Namespace = "AppNamespace.Model", Association = span },
            };
            host.SetupGet(h => h.DefaultInheritedChunks)
                .Returns(defaultChunks);

            var utility = host.Object.ChunkInheritanceUtility;

            // Act
            var chunks = utility.GetInheritedChunks(@"x:\myapproot\views\home\Index.cshtml");

            // Assert
            Assert.Equal(4, chunks.Count);
            var injectChunk = Assert.IsType<InjectChunk>(chunks[1]);
            Assert.Equal("DifferentHelper<TModel>", injectChunk.TypeName);
            Assert.Equal("Html", injectChunk.MemberName);

            Assert.Same(defaultChunks[0], chunks[2]);
            Assert.Same(defaultChunks[1], chunks[3]);
        }

        [Fact]
        public void GetInheritedChunks_UsesViewStartChunksFromCompilerCache()
        {
            // Arrange
            var fileSystem = new TestFileSystem();
            var viewStart = @"views\_viewstart.cshtml";
            var codeTree = new CodeTree();
            codeTree.Chunks.Add(new InjectChunk("sometype", "someproperty"));
            codeTree.Chunks.Add(new UsingChunk { Namespace = "someusing" });

            fileSystem.AddFile(viewStart,
                               "@inject DifferentHelper<TModel> Html");
            var cache = new Mock<ICompilerCache>();
            cache.Setup(f => f.GetOrAddMetadata(It.Is<RelativeFileInfo>(i => i.RelativePath == viewStart),
                                                It.IsAny<Func<RelativeFileInfo, CompilationResult>>(),
                                                ChunkInheritanceUtility.CodeTreeMetadataCacheKey,
                                                It.IsAny<Func<object>>()))
                 .Returns(codeTree)
                 .Verifiable();
            var host = new MvcRazorHost(fileSystem, new NullCompilationService(), cache.Object);

            // Act
            var chunks = host.ChunkInheritanceUtility.GetInheritedChunks(@"views\home\Index.cshtml");

            // Assert
            Assert.Equal(5, chunks.Count);
            Assert.Same(codeTree.Chunks[0], chunks[0]);
            Assert.Same(codeTree.Chunks[1], chunks[1]);
        }
    }
}