// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#if ASPNET50
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Moq;
using Xunit;

namespace Microsoft.AspNet.Mvc.Test
{
    public class ActionFilterAttributeTests
    {
        [Fact]
        public async Task ActionFilterAttribute_ActionFilter_SettingResult_ShortCircuits()
        {
            await ActionFilter_SettingResult_ShortCircuits(new Mock<ActionFilterAttribute>());
        }

        [Fact]
        public async Task ActionAttributeFilter_ActionFilter_Calls_OnActionExecuted()
        {
            await ActionFilter_Calls_OnActionExecuted(new Mock<ActionFilterAttribute>());
        }

        // This is used as a 'common' test method for ActionFilterAttribute and Controller
        public static async Task ActionFilter_Calls_OnActionExecuted(Mock mock)
        {
            // Arrange
            mock.As<IAsyncActionFilter>()
                .Setup(f => f.OnActionExecutionAsync(
                    It.IsAny<ActionExecutingContext>(),
                    It.IsAny<ActionExecutionDelegate>()))
                .CallBase();

            mock.As<IActionFilter>()
                .Setup(f => f.OnActionExecuting(It.IsAny<ActionExecutingContext>()))
                .Verifiable();

            mock.As<IActionFilter>()
                .Setup(f => f.OnActionExecuted(It.IsAny<ActionExecutedContext>()))
                .Verifiable();

            var context = CreateActionExecutingContext(mock.As<IFilter>().Object);
            var next = new ActionExecutionDelegate(() => Task.FromResult(CreateActionExecutedContext(context)));

            // Act
            await mock.As<IAsyncActionFilter>().Object.OnActionExecutionAsync(context, next);

            // Assert
            Assert.Null(context.Result);

            mock.As<IActionFilter>()
                .Verify(f => f.OnActionExecuting(It.IsAny<ActionExecutingContext>()), Times.Once());

            mock.As<IActionFilter>()
                .Verify(f => f.OnActionExecuted(It.IsAny<ActionExecutedContext>()), Times.Once());
        }

        // This is used as a 'common' test method for ActionFilterAttribute and Controller
        public static async Task ActionFilter_SettingResult_ShortCircuits(Mock mock)
        {
            // Arrange
            mock.As<IAsyncActionFilter>()
                .Setup(f => f.OnActionExecutionAsync(
                    It.IsAny<ActionExecutingContext>(),
                    It.IsAny<ActionExecutionDelegate>()))
                .CallBase();

            mock.As<IActionFilter>()
                .Setup(f => f.OnActionExecuting(It.IsAny<ActionExecutingContext>()))
                .Callback<ActionExecutingContext>(c =>
                {
                    mock.ToString();
                    c.Result = new NoOpResult();
                });

            mock.As<IActionFilter>()
                .Setup(f => f.OnActionExecuted(It.IsAny<ActionExecutedContext>()))
                .Verifiable();

            var context = CreateActionExecutingContext(mock.As<IFilter>().Object);
            var next = new ActionExecutionDelegate(() => { throw null; }); // This won't run

            // Act
            await mock.As<IAsyncActionFilter>().Object.OnActionExecutionAsync(context, next);

            // Assert
            Assert.IsType<NoOpResult>(context.Result);

            mock.As<IActionFilter>()
                .Verify(f => f.OnActionExecuted(It.IsAny<ActionExecutedContext>()), Times.Never());
        }

        [Fact]
        public async Task ActionAttributeFilter_ResultFilter_Calls_OnResultExecuted()
        {
            await ResultFilter_Calls_OnResultExecuted(new Mock<ActionFilterAttribute>());
        }

        [Fact]
        public async Task ActionFilterAttribute_ResultFilter_SettingResult_DoesNotShortCircuit()
        {
            await ResultFilter_SettingResult_DoesNotShortCircuit(new Mock<ActionFilterAttribute>());
        }

        [Fact]
        public async Task ActionFilterAttribute_ResultFilter_SettingCancel_ShortCircuits()
        {
            await ResultFilter_SettingCancel_ShortCircuits(new Mock<ActionFilterAttribute>());
        }

        // This is used as a 'common' test method for ActionFilterAttribute and ResultFilterAttribute
        public static async Task ResultFilter_Calls_OnResultExecuted(Mock mock)
        {
            // Arrange
            mock.As<IAsyncResultFilter>()
                .Setup(f => f.OnResultExecutionAsync(
                    It.IsAny<ResultExecutingContext>(),
                    It.IsAny<ResultExecutionDelegate>()))
                .CallBase();

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()))
                .Verifiable();

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()))
                .Verifiable();

            var context = CreateResultExecutingContext(mock.As<IFilter>().Object);
            var next = new ResultExecutionDelegate(() => Task.FromResult(CreateResultExecutedContext(context)));

            // Act
            await mock.As<IAsyncResultFilter>().Object.OnResultExecutionAsync(context, next);

            // Assert
            Assert.False(context.Cancel);

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()), Times.Once());

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()), Times.Once());
        }

        // This is used as a 'common' test method for ActionFilterAttribute and ResultFilterAttribute
        public static async Task ResultFilter_SettingResult_DoesNotShortCircuit(Mock mock)
        {
            // Arrange
            mock.As<IAsyncResultFilter>()
                .Setup(f => f.OnResultExecutionAsync(
                    It.IsAny<ResultExecutingContext>(),
                    It.IsAny<ResultExecutionDelegate>()))
                .CallBase();

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()))
                .Callback<ResultExecutingContext>(c =>
                {
                    mock.ToString();
                    c.Result = new NoOpResult();
                });

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()))
                .Verifiable();

            var context = CreateResultExecutingContext(mock.As<IFilter>().Object);
            var next = new ResultExecutionDelegate(() => Task.FromResult(CreateResultExecutedContext(context)));

            // Act
            await mock.As<IAsyncResultFilter>().Object.OnResultExecutionAsync(context, next);

            // Assert
            Assert.False(context.Cancel);

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()), Times.Once());

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()), Times.Once());
        }

        // This is used as a 'common' test method for ActionFilterAttribute and ResultFilterAttribute
        public static async Task ResultFilter_SettingCancel_ShortCircuits(Mock mock)
        {
            // Arrange
            mock.As<IAsyncResultFilter>()
                .Setup(f => f.OnResultExecutionAsync(
                    It.IsAny<ResultExecutingContext>(),
                    It.IsAny<ResultExecutionDelegate>()))
                .CallBase();

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()))
                .Callback<ResultExecutingContext>(c =>
                {
                    mock.ToString();
                    c.Cancel = true;
                });

            mock.As<IResultFilter>()
                .Setup(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()))
                .Verifiable();

            var context = CreateResultExecutingContext(mock.As<IFilter>().Object);
            var next = new ResultExecutionDelegate(() => { throw null; }); // This won't run

            // Act
            await mock.As<IAsyncResultFilter>().Object.OnResultExecutionAsync(context, next);

            // Assert
            Assert.True(context.Cancel);

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuting(It.IsAny<ResultExecutingContext>()), Times.Once());

            mock.As<IResultFilter>()
                .Verify(f => f.OnResultExecuted(It.IsAny<ResultExecutedContext>()), Times.Never());
        }

        public static ActionExecutingContext CreateActionExecutingContext(IFilter filter)
        {
            return new ActionExecutingContext(
                CreateActionContext(),
                new IFilter[] { filter, },
                new Dictionary<string, object>());
        }

        public static ActionExecutedContext CreateActionExecutedContext(ActionExecutingContext context)
        {
            return new ActionExecutedContext(context, context.Filters)
            {
                Result = context.Result,
            };
        }

        public static ResultExecutingContext CreateResultExecutingContext(IFilter filter)
        {
            return new ResultExecutingContext(
                CreateActionContext(),
                new IFilter[] { filter, },
                new NoOpResult());
        }

        public static ResultExecutedContext CreateResultExecutedContext(ResultExecutingContext context)
        {
            return new ResultExecutedContext(context, context.Filters, context.Result);
        }

        public static ActionContext CreateActionContext()
        {
            return new ActionContext(Mock.Of<HttpContext>(), new RouteData(), new ActionDescriptor());
        }

        private class NoOpResult : IActionResult
        {
            public Task ExecuteResultAsync(ActionContext context)
            {
                return Task.FromResult(true);
            }
        }
    }
}
#endif
