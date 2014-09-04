// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.Framework.DependencyInjection;
using System.Linq;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents the <see cref="IControllerActivator"/> that is registered by default.
    /// </summary>
    public class DefaultControllerActivator : IControllerActivator
    {
        private readonly Func<Type, PropertyActivator<ActionContext>[]> _getPropertiesToActivate;
        private readonly IReadOnlyDictionary<Type, Func<ActionContext, object>> _valueAccessorLookup;
        private readonly ConcurrentDictionary<Type, PropertyActivator<ActionContext>[]> _injectActions;
        private readonly IActionBindingContextProvider _bindingContextProvider;
        /// <summary>
        /// Initializes a new instance of the DefaultControllerActivator class.
        /// </summary>
        public DefaultControllerActivator(
            [NotNull] IActionBindingContextProvider bindingContextProvider)
        {
            _bindingContextProvider = bindingContextProvider;
            _valueAccessorLookup = CreateValueAccessorLookup();
            _injectActions = new ConcurrentDictionary<Type, PropertyActivator<ActionContext>[]>();
            _getPropertiesToActivate = type =>
                PropertyActivator<ActionContext>.GetPropertiesToActivate(type,
                                                                         null,
                                                                         CreateActivateInfo);
        }

        public DefaultControllerActivator()
        {
            _valueAccessorLookup = CreateValueAccessorLookup();
            _injectActions = new ConcurrentDictionary<Type, PropertyActivator<ActionContext>[]>();
            _getPropertiesToActivate = type =>
                PropertyActivator<ActionContext>.GetPropertiesToActivate(type,
                                                                         null,
                                                                         CreateActivateInfo);
        }

        /// <summary>
        /// Activates the specified controller by using the specified action context.
        /// </summary>
        /// <param name="controller">The controller to activate.</param>
        /// <param name="context">The context of the executing action.</param>
        public void Activate([NotNull] object controller, [NotNull] ActionContext context)
        {
            var controllerType = controller.GetType();
            var controllerTypeInfo = controllerType.GetTypeInfo();
            if (controllerTypeInfo.IsValueType)
            {
                var message = Resources.FormatValueTypesCannotBeActivated(GetType().FullName);
                throw new InvalidOperationException(message);
            }
            var propertiesToActivate = _injectActions.GetOrAdd(controllerType,
                                                               _getPropertiesToActivate);

            //for (var i = 0; i < propertiesToActivate.Length; i++)
            //{
            //    var activateInfo = propertiesToActivate[i];
            //    activateInfo.Activate(controller, context);
            //}
        }

        protected virtual IReadOnlyDictionary<Type, Func<ActionContext, object>> CreateValueAccessorLookup()
        {
            var dictionary = new Dictionary<Type, Func<ActionContext, object>>
            {
                { typeof(ActionContext), (context) => context },
                { typeof(HttpContext), (context) => context.HttpContext },
                { typeof(HttpRequest), (context) => context.HttpContext.Request },
                { typeof(HttpResponse), (context) => context.HttpContext.Response },
                {
                    typeof(ViewDataDictionary),
                    (context) =>
                    {
                        var serviceProvider = context.HttpContext.RequestServices;
                        return new ViewDataDictionary(
                            serviceProvider.GetService<IModelMetadataProvider>(),
                            context.ModelState);
                    }
                }
            };
            return dictionary;
        }

        private PropertyActivator<ActionContext> CreateActivateInfo(
            PropertyInfo property)
        {
            Func<ActionContext, object> valueAccessor;
            if (!_valueAccessorLookup.TryGetValue(property.PropertyType, out valueAccessor))
            {
                valueAccessor = (actionContext =>
                {
                    var actionBindingContext = _bindingContextProvider.GetActionBindingContextAsync(actionContext).Result;
                    var metadataProvider = actionBindingContext.MetadataProvider;

                    // First get data for the controller properties. 
                        var modelMetadata = metadataProvider.GetMetadataForType(
                                modelAccessor: null,
                                modelType: property.PropertyType);

                        var uberContext = new UberBindingContext()
                        {
                            //ActionContext = actionContext,
                            HttpContext = actionContext.HttpContext,
                            ModelState = actionContext.ModelState,
                            ModelName = property.Name,
                            ModelMetadata = modelMetadata,
                            ModelBinder = actionBindingContext.ModelBinder,
                            ValueProvider = actionBindingContext.ValueProvider,
                            ValidatorProvider = actionBindingContext.ValidatorProvider,
                            MetadataProvider = metadataProvider,
                            // FallbackToEmptyPrefix = true
                        };

                    // It is possible to provide a chained list which can be used to bind the object. 
                    // For now choosing the first one.
                    var uberBindingAttribute = property.GetCustomAttributes()
                                                       .OfType<UberBindingAttribute>()
                                                       .FirstOrDefault();
                    var binding = uberBindingAttribute?.GetBinding(new TypeDescriptor() { Type = modelMetadata.ModelType });
                    binding = binding ?? new ModelBindingWrapper();

                    binding.BindAsync(uberContext).Wait();
                    return uberContext.Model;
                });
            }

            return new PropertyActivator<ActionContext>(property, valueAccessor);
        }
    }
}
