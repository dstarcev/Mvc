// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc
{
    public class ModelBindingWrapper : IUberBinding
    {
        public async Task BindAsync(UberBindingContext context)
        {
            var modelBindingContext = new ModelBindingContext
            {
                ModelName = context.ModelName,
                ModelState = context.ModelState,
                ModelMetadata = context.ModelMetadata,
                ModelBinder = context.ModelBinder,
                ValueProvider = context.ValueProvider,
                ValidatorProvider = context.ValidatorProvider,
                MetadataProvider = context.MetadataProvider,
                HttpContext = context.HttpContext,
            };

            await context.ModelBinder.BindModelAsync(modelBindingContext);

            // Copy over the value.
            context.Model = modelBindingContext.Model;
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }
}
