// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    public class UberBindingModelMetadata : ModelMetadata
    {
        public UberBindingModelMetadata([NotNull] IModelMetadataProvider provider, Type containerType, Func<object> modelAccessor, [NotNull]Type modelType, string propertyName, IUberBinding binding) : base(provider, containerType, modelAccessor, modelType, propertyName)
        {
            Binding = binding;
        }

        public UberBindingModelMetadata([NotNull] IModelMetadataProvider provider, IEnumerable<Attribute> attributes, Type containerType, Type modelType, string propertyName)
            : base(provider, containerType, null, modelType, propertyName)
        {
            var bindingAttributes = attributes.OfType<UberBindingAttribute>();
            var descriptor = new PropertyDescriptor()
            {
                Name = propertyName,
                Type = modelType,
                BindingAttributes = bindingAttributes
            };

            Binding = GetBinding(descriptor);
        }

        public IUberBinding Binding { get; set; }

        private IUberBinding GetBinding(Descriptor descriptor)
        {
            foreach (var uberBinding in descriptor.BindingAttributes)
            {
                var binding = uberBinding?.GetBinding(descriptor);
                binding = binding ?? new ModelBindingWrapper();
                //if(binding.CanBind(descriptor.Type))
                //{
                //    return binding;
                //}

                return binding;
            }

            // If no specific binding was found, look at the type definition and see. 
            var customAttributes = Attribute.GetCustomAttributes(descriptor.Type).OfType<UberBindingAttribute>().FirstOrDefault();
            return customAttributes?.GetBinding(descriptor) ?? new ModelBindingWrapper();
        }
    }
}
