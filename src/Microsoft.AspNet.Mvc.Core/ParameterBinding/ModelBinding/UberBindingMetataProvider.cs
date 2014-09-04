// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    public class UberBindingMetataProvider : AssociatedMetadataProvider<UberBindingModelMetadata>
    {
        protected override UberBindingModelMetadata CreateMetadataFromPrototype(UberBindingModelMetadata prototype, Func<object> modelAccessor)
        {
            return new UberBindingModelMetadata(this, prototype.ContainerType, modelAccessor, prototype.ModelType, prototype.PropertyName, prototype.Binding);
        }

        protected override UberBindingModelMetadata CreateMetadataPrototype(IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
        {
            return new UberBindingModelMetadata(this, attributes, containerType, modelType, propertyName);
        }
    }
}
