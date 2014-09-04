// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    public class ValueAccessorBinding : IUberBinding
    {
        private readonly IDictionary<Type, Func<HttpContext, object>> _valueAccessorLookup
            = new Dictionary<Type, Func<HttpContext, object>>();
        private Func<ActionContext, object> _valueAccessor;

        public void AddOrUpdateAccessor(Type type, Func<HttpContext, object> valueAccesor)
        {
            _valueAccessorLookup.Add(type, valueAccesor);
        }

        public Task BindAsync(UberBindingContext context)
        {
            if(_valueAccessorLookup.TryGetValue(context.ModelMetadata.ModelType, out var accessor))
            {
                context.Model = _valueAccessorLookup[context.ModelMetadata.ModelType](context.HttpContext);
                return Task.FromResult<bool>(true);
            }

            return Task.FromResult<bool>(false);
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }
}
