// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents an <see cref="IBinderMetadata"/> with or without an actual binder or a binder marker.
    /// </summary>
    public class BinderItem
    {
        public BinderItem(IBinderMetadata binderMetadata)
        {
            BinderMetadata = binderMetadata;
        }

        public IModelBinder Binder { get; set; }

        public IBinderMarker BinderMarker { get; set; }

        public IBinderMetadata BinderMetadata { get; private set; }
    }
}
