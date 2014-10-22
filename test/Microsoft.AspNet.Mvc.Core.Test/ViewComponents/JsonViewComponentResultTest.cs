// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.PipelineCore;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class JsonViewComponentResultTest
    {
        [Fact]
        public void Execute_SerializesData_UsingResolvedFormater()
        {
            // Arrange
            var view = Mock.Of<IView>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(p => p.GetService(typeof(JsonOutputFormatter)))
                .Returns(new JsonOutputFormatter());

            var buffer = new MemoryStream();

            var result = new JsonViewComponentResult(1);
            var viewComponentContext = GetViewComponentContext(view, buffer);
            viewComponentContext.ViewContext.HttpContext.RequestServices = serviceProvider.Object;

            // Act
            result.Execute(viewComponentContext);
            buffer.Position = 0;

            // Assert
            Assert.Equal("1", new StreamReader(buffer).ReadToEnd());
        }

        [Fact]
        public void Execute_Throws_IfNoFormatterCanBeResolved()
        {
            // Arrange
            var expected = "TODO: No service for type 'Microsoft.AspNet.Mvc.JsonOutputFormatter'" +
                " has been registered.";

            var view = Mock.Of<IView>();

            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var buffer = new MemoryStream();

            var result = new JsonViewComponentResult(1);
            var viewComponentContext = GetViewComponentContext(view, buffer);
            viewComponentContext.ViewContext.HttpContext.RequestServices = serviceProvider;

            // Act
            var ex = Assert.Throws<Exception>(() => result.Execute(viewComponentContext));

            // Assert
            Assert.Equal(expected, ex.Message);
        }

        private static ViewComponentContext GetViewComponentContext(IView view, Stream stream)
        {
            var actionContext = new ActionContext(new RouteContext(new DefaultHttpContext()), new ActionDescriptor());
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider());
            var viewContext = new ViewContext(actionContext, view, viewData, TextWriter.Null);
            var writer = new StreamWriter(stream) { AutoFlush = true };
            var viewComponentContext = new ViewComponentContext(typeof(object).GetTypeInfo(), viewContext, writer);
            return viewComponentContext;
        }
    }
}