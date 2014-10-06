// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Mvc
{
    public class DefaultBinderProvider : INestedProvider<MarkerProviderContext>
    {
        private IServiceProvider _serviceProvider;

        public DefaultBinderProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public int Order
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Invoke(MarkerProviderContext context, Action callNext)
        {
            throw new NotImplementedException();
        }

        public IModelBinder ProvideBinder(MarkerProviderContext providerContext)
        {
            var markerMetadata = providerContext.Result;
            var markerFactory = markerMetadata as IBinderFactory;
            if (markerFactory != null)
            {
                var binderMarker = markerFactory.CreateInstance(_serviceProvider);
                return binderMarker;
            }

            // TODO: throw if this is neither.
            return null;
        }
    }
}
