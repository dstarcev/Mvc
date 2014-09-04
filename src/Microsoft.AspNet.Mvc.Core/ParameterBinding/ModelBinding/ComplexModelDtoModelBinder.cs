// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.ModelBinding.Internal;

namespace Microsoft.AspNet.Mvc.ModelBinding
{
    public sealed class ComplexModelDtoModelBinder2 : IModelBinder
    {
        public async Task<bool> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType == typeof(ComplexModelDto))
            {
                //ModelBindingHelper.ValidateBindingContext(bindingContext,
                //                                          typeof(ComplexModelDto),
                //                                          allowNullModel: false);

                var dto = (ComplexModelDto)bindingContext.Model;
                foreach (var propertyMetadata in dto.PropertyMetadata)
                {
                    var propertyBindingContext = new ModelBindingContext(bindingContext)
                    {
                        ModelMetadata = propertyMetadata,
                        ModelName = ModelBindingHelper.CreatePropertyModelName(bindingContext.ModelName,
                                                                               propertyMetadata.PropertyName)
                    };

                    var metadata = propertyMetadata as UberBindingModelMetadata;
                    if (metadata != null)
                    {
                        var uberContext = UberBindingContextHelper.GetUberBindingContext(propertyBindingContext);
                        await metadata.Binding.BindAsync(uberContext);

                        // TODO: If we can't bind, then leave the result missing (don't add a null).
                        var result = new ComplexModelDtoResult(uberContext.Model,
                                                               propertyBindingContext.ValidationNode);
                        dto.Results[propertyMetadata] = result;
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
