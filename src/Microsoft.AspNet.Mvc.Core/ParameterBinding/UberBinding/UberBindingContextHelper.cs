// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc
{
    public class UberBindingContextHelper
    {
      public static UberBindingContext GetUberBindingContext(ModelBindingContext modelBindingContext)
        {
            // Todo: Since there is no action context with model binding, we would have to leave that out for now. 
            // this means that anything depending on action context ( like service injection, from headers etc. ) would not work.
            return new UberBindingContext
            {
                ModelName = modelBindingContext.ModelName,
                ModelState = modelBindingContext.ModelState,
                ModelMetadata = modelBindingContext.ModelMetadata,
                ModelBinder = modelBindingContext.ModelBinder,
                ValueProvider = modelBindingContext.ValueProvider,
                ValidatorProvider = modelBindingContext.ValidatorProvider,
                MetadataProvider = modelBindingContext.MetadataProvider,
                HttpContext = modelBindingContext.HttpContext
            };
        }
    }
}
