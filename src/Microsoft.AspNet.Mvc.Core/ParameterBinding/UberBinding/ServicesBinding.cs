// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc
{
    public class ServicesBinding : IUberBinding
    {
        public Task BindAsync(UberBindingContext context)
        {
            var serviceProvider = context.HttpContext.RequestServices;
            context.Model = serviceProvider.GetService(context.ModelMetadata.ModelType);
            return Task.FromResult<bool>(true);
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }
}
