// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc
{
    public class HeaderBinding : IUberBinding
    {
        private string _headerKey;
        
        public HeaderBinding(string key)
        {
            _headerKey = key;
        }

        public Task BindAsync(UberBindingContext context)
        {
            var header = context.HttpContext.Request.Headers[_headerKey];
            context.Model = header;
            return Task.FromResult<bool>(true);
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(string);
        }
    }
}
