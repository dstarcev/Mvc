// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    public class DefaultBinderMarkerProvider : IBinderMarkerProvider
    {
        private IServiceProvider _serviceProvider;

        public DefaultBinderMarkerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IBinderMarker ProvideMarker(MarkerProviderContext providerContext)
        {
            var markerMetadata = providerContext.MarkerMetadata;
            var markerFactory = markerMetadata as IBinderFactory;
            if (markerFactory != null)
            {
                var binderMarker = markerFactory.CreateInstance(_serviceProvider);
                return binderMarker;
            }

            // TODO: throw if this is neither.
            var marker = markerMetadata as IBinderMarker;
            if (marker == null)
            {
                // always introduce a marker which fetches from query.
                return new FromQueryAttribute();
            }

            return marker;
        }
    }
}
