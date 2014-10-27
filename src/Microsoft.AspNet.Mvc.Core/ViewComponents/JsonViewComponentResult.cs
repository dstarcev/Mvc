// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using System.Linq;

namespace Microsoft.AspNet.Mvc
{
    public class JsonViewComponentResult : IViewComponentResult
    {
        public JsonViewComponentResult([NotNull] object data)
        {
            Value = data;
        }

        public JsonViewComponentResult([NotNull] object data, JsonOutputFormatter formatter)
        {
            Value = data;
            Formatter = formatter;
        }

        public object Value { get; private set; }

        public JsonOutputFormatter Formatter { get; private set; }

        public void Execute([NotNull] ViewComponentContext context)
        {
            var formatter = Formatter ?? ResolveFormatter(context);
            formatter.WriteObject(context.Writer, Value);
        }

        private static JsonOutputFormatter ResolveFormatter(ViewComponentContext context)
        {
            var services = context.ViewContext.HttpContext.RequestServices;

            var provider = services.GetRequiredService<IOutputFormattersProvider>();

            var formatter = (JsonOutputFormatter)provider
                .OutputFormatters
                .FirstOrDefault(f => f is JsonOutputFormatter);

            return formatter ?? services.GetRequiredService<JsonOutputFormatter>();
        }

        public Task ExecuteAsync([NotNull] ViewComponentContext context)
        {
            Execute(context);
            return Task.FromResult(true);
        }
    }
}
