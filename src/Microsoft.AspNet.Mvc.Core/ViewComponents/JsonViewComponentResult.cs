// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class JsonViewComponentResult : IViewComponentResult
    {
        public JsonViewComponentResult([NotNull] object data)
        {
            Value = data;
        }

        public object Value { get; set; }

        public void Execute([NotNull] ViewComponentContext context)
        {
            var formatter = ResolveFormatter(context);
            formatter.WriteObject(context.Writer, Value);
        }

        private static JsonOutputFormatter ResolveFormatter(ViewComponentContext context)
        {
            return context.ViewContext.HttpContext.RequestServices.GetRequiredService<JsonOutputFormatter>();
        }

        public Task ExecuteAsync([NotNull] ViewComponentContext context)
        {
            Execute(context);
            return Task.FromResult(true);
        }
    }
}
