// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class BodyBinding : IUberBinding
    {
        public async Task BindAsync(UberBindingContext context)
        {
            var formatterContext = new InputFormatterContext(context.ActionContext, context.ModelMetadata.ModelType);
            var formatterSelector = context.HttpContext.RequestServices.GetService<IInputFormatterSelector>();

            var selectedFormatter = formatterSelector.SelectFormatter(formatterContext);
            context.Model = await selectedFormatter.ReadAsync(formatterContext);
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }
}
