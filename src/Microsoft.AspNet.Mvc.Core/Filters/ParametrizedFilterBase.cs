// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc
{
    public abstract class ParametrizedFilterBase<TParameters> : IParametrizedFilter<TParameters>
    {
        public virtual void OnException(ExceptionContext context, TParameters parameters)
        {
        }

        public virtual Task OnExceptionAsync(ExceptionContext context, TParameters data)
        {
            return Task.CompletedTask;
        }

        public virtual void OnAuthorization(AuthorizationContext context, TParameters data)
        {
        }

        public virtual Task OnAuthorizationAsync(AuthorizationContext context, TParameters data)
        {
            return Task.CompletedTask;
        }

        public virtual void OnActionExecuting(ActionExecutingContext context, TParameters data)
        {
        }

        public virtual void OnActionExecuted(ActionExecutedContext context, TParameters data)
        {
        }

        public virtual Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next,
            TParameters data)
        {
            return next();
        }

        public virtual void OnResultExecuting(ResultExecutingContext context, TParameters data)
        {
        }

        public virtual void OnResultExecuted(ResultExecutedContext context, TParameters data)
        {
        }

        public virtual Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next,
            TParameters data)
        {
            return next();
        }
    }
}