// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Mvc
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public abstract class UberBindingAttribute : Attribute
    {
        public abstract IUberBinding GetBinding(Descriptor descriptor);
    }

    public interface IUberBinding
    {
        Task BindAsync(UberBindingContext context);

        bool CanBind(Type modelType);
    }

    public class Descriptor
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public ActionDescriptor ActionDescriptor { get; set; }
    }

    public class UberBindingContext
    {
        public ActionContext ActionContext { get; set; }
        public IModelMetadataProvider MetadataProvider { get; internal set; }
        public object Model { get; set; }
        public IModelBinder ModelBinder { get; internal set; }
        public ModelMetadata ModelMetadata { get; internal set; }
        public string ModelName { get; internal set; }
        public IModelValidatorProvider ValidatorProvider { get; internal set; }
        public IValueProvider ValueProvider { get; internal set; }
    }

    public class FromHeaderAttribute : UberBindingAttribute
    {
        private string _headerKey;

        public FromHeaderAttribute(string key)
        {
            _headerKey = key;
        }

        public override IUberBinding GetBinding(Descriptor descriptor)
        {
            return new HeaderBinding(_headerKey);
        }
    }

    public class HeaderBinding : IUberBinding
    {
        private string _headerKey;
        
        public HeaderBinding(string key)
        {
            _headerKey = key;
        }

        public Task BindAsync(UberBindingContext context)
        {
            var header = context.ActionContext.HttpContext.Request.Headers[_headerKey];
            context.Model = header;
            return Task.FromResult<bool>(true);
        }

        public bool CanBind(Type modelType)
        {
            return modelType == typeof(string);
        }
    }

    public class ServicesBinding : IUberBinding
    {
        public Task BindAsync(UberBindingContext context)
        {
            var serviceProvider = context.ActionContext.HttpContext.RequestServices;
            context.Model = serviceProvider.GetService(context.ModelMetadata.ModelType);
            return Task.FromResult<bool>(true);
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }

    public class UberBinding : IUberBinding
    {
        public async Task BindAsync(UberBindingContext context)
        {
            var modelBindingContext = new ModelBindingContext
            {
                ModelName = context.ModelName,
                ModelState = context.ActionContext.ModelState,
                ModelMetadata = context.ModelMetadata,
                ModelBinder = context.ModelBinder,
                ValueProvider = context.ValueProvider,
                ValidatorProvider = context.ValidatorProvider,
                //MetadataProvider = metadataProvider,
                HttpContext = context.ActionContext.HttpContext,
            };

            await context.ModelBinder.BindModelAsync(modelBindingContext);

            // Copy over the value.
            context.Model = modelBindingContext.Model;
        }

        public bool CanBind(Type modelType)
        {
            throw new NotImplementedException();
        }
    }
}
