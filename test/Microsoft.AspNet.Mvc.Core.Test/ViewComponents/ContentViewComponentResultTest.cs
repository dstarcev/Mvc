// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.PipelineCore;
using Microsoft.AspNet.Routing;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc
{
    public class ContentViewComponentResultTest
    {
        [Fact]
        public void Execute_WritesData_Encoded()
        {
            // Arrange
            var view = Mock.Of<IView>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(p => p.GetService(typeof(JsonOutputFormatter)))
                .Returns(new JsonOutputFormatter());

            var buffer = new MemoryStream();

            var result = new ContentViewComponentResult("<Test />");
            var viewComponentContext = GetViewComponentContext(view, buffer);
            viewComponentContext.ViewContext.HttpContext.RequestServices = serviceProvider.Object;

            // Act
            result.Execute(viewComponentContext);
            buffer.Position = 0;

            // Assert
            Assert.Equal("&lt;Test /&gt;", new StreamReader(buffer).ReadToEnd());
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