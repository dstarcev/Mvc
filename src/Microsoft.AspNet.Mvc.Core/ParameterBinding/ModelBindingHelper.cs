﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.AspNet.Mvc.ModelBinding;
using Microsoft.AspNet.Mvc.Rendering.Expressions;

namespace Microsoft.AspNet.Mvc
{
    public static class ModelBindingHelper
    {
        /// <summary>
        /// Updates the specified model instance using the specified binder and value provider and 
        /// executes validation using the specified sequence of validator providers.
        /// </summary>
        /// <typeparam name="TModel">The type of the model object.</typeparam>
        /// <param name="model">The model instance to update.</param>
        /// <param name="prefix">The prefix to use when looking up values in the value provider.</param>
        /// <param name="httpContext">The context for the current executing request.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> used for maintaining state and 
        /// results of model-binding validation.</param>
        /// <param name="metadataProvider">The provider used for reading metadata for the model type.</param>
        /// <param name="modelBinder">The model binder used for binding.</param>
        /// <param name="valueProvider">The value provider used for looking up values.</param>
        /// <param name="validatorProvider">The validator provider used for executing validation on the model
        /// instance.</param>
        /// <returns>A <see cref="Task"/> with a value representing if the the update is successful.</returns>
        public static async Task<bool> TryUpdateModelAsync<TModel>(
                [NotNull] TModel model,
                [NotNull] string prefix,
                [NotNull] HttpContext httpContext,
                [NotNull] ModelStateDictionary modelState,
                [NotNull] IModelMetadataProvider metadataProvider,
                [NotNull] IModelBinder modelBinder,
                [NotNull] IValueProvider valueProvider,
                [NotNull] IModelValidatorProvider validatorProvider)
            where TModel : class
        {
            // Includes everything by default.
            return await TryUpdateModelAsync(
                model,
                prefix,
                httpContext,
                modelState,
                metadataProvider,
                modelBinder,
                valueProvider,
                validatorProvider,
                predicate: (context, propertyName) => true);
        }

        /// <summary>
        /// Updates the specified model instance using the specified binder and value provider and 
        /// executes validation using the specified sequence of validator providers.
        /// </summary>
        /// <typeparam name="TModel">The type of the model object.</typeparam>
        /// <param name="model">The model instance to update.</param>
        /// <param name="prefix">The prefix to use when looking up values in the value provider.</param>
        /// <param name="httpContext">The context for the current executing request.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> used for maintaining state and 
        /// results of model-binding validation.</param>
        /// <param name="metadataProvider">The provider used for reading metadata for the model type.</param>
        /// <param name="modelBinder">The model binder used for binding.</param>
        /// <param name="valueProvider">The value provider used for looking up values.</param>
        /// <param name="validatorProvider">The validator provider used for executing validation on the model
        /// instance.</param>
        /// <param name="includeExpressions">Expression(s) which represent top level properties 
        /// which need to be included for the current model.</param>
        /// <returns>A <see cref="Task"/> with a value representing if the the update is successful.</returns>
        public static async Task<bool> TryUpdateModelAsync<TModel>(
               [NotNull] TModel model,
               [NotNull] string prefix,
               [NotNull] HttpContext httpContext,
               [NotNull] ModelStateDictionary modelState,
               [NotNull] IModelMetadataProvider metadataProvider,
               [NotNull] IModelBinder modelBinder,
               [NotNull] IValueProvider valueProvider,
               [NotNull] IModelValidatorProvider validatorProvider,
               [NotNull] params Expression<Func<TModel, object>>[] includeExpressions)
           where TModel : class
        {
            var includePredicates = GetIncludePredicates(prefix, includeExpressions).ToArray();
            Func<ModelBindingContext, string, bool> predicate =
                (bindingContext, modelName) =>
                    includePredicates.Any(includePredicate => includePredicate(bindingContext, modelName));

            return await TryUpdateModelAsync(
               model,
               prefix,
               httpContext,
               modelState,
               metadataProvider,
               modelBinder,
               valueProvider,
               validatorProvider,
               predicate: predicate);
        }

        /// <summary>
        /// Updates the specified model instance using the specified binder and value provider and 
        /// executes validation using the specified sequence of validator providers.
        /// </summary>
        /// <typeparam name="TModel">The type of the model object.</typeparam>
        /// <param name="model">The model instance to update.</param>
        /// <param name="prefix">The prefix to use when looking up values in the value provider.</param>
        /// <param name="httpContext">The context for the current executing request.</param>
        /// <param name="modelState">The <see cref="ModelStateDictionary"/> used for maintaining state and 
        /// results of model-binding validation.</param>
        /// <param name="metadataProvider">The provider used for reading metadata for the model type.</param>
        /// <param name="modelBinder">The model binder used for binding.</param>
        /// <param name="valueProvider">The value provider used for looking up values.</param>
        /// <param name="validatorProvider">The validator provider used for executing validation on the model
        /// instance.</param>
        /// <param name="predicate">A predicate which can be used to 
        /// filter properties(for inclusion/exclusion) at runtime.</param>
        /// <returns>A <see cref="Task"/> with a value representing if the the update is successful.</returns>
        public static async Task<bool> TryUpdateModelAsync<TModel>(
               [NotNull] TModel model,
               [NotNull] string prefix,
               [NotNull] HttpContext httpContext,
               [NotNull] ModelStateDictionary modelState,
               [NotNull] IModelMetadataProvider metadataProvider,
               [NotNull] IModelBinder modelBinder,
               [NotNull] IValueProvider valueProvider,
               [NotNull] IModelValidatorProvider validatorProvider,
               [NotNull] Func<ModelBindingContext, string, bool> predicate)
           where TModel : class
        {
            var modelMetadata = metadataProvider.GetMetadataForType(
                modelAccessor: null,
                modelType: typeof(TModel));

            var modelBindingContext = new ModelBindingContext
            {
                ModelMetadata = modelMetadata,
                ModelName = prefix,
                Model = model,
                ModelState = modelState,
                ModelBinder = modelBinder,
                ValueProvider = valueProvider,
                ValidatorProvider = validatorProvider,
                MetadataProvider = metadataProvider,
                FallbackToEmptyPrefix = true,
                HttpContext = httpContext,
                PropertyFilter = predicate
            };

            if (await modelBinder.BindModelAsync(modelBindingContext))
            {
                return modelState.IsValid;
            }

            return false;
        }

        private static IEnumerable<Func<ModelBindingContext, string, bool>> GetIncludePredicates<TModel>
            (string prefix, IEnumerable<Expression<Func<TModel, object>>> expressions)
        {
            foreach (var expression in expressions)
            {
                var propertyName = GetPropertyName(expression.Body);
                var property = CreatePropertyModelName(prefix, propertyName);

                Func<ModelBindingContext, string, bool> predicate =
                (context, modelPropertyName) =>
                {
                    var fullPropertyName = CreatePropertyModelName(context.ModelName, modelPropertyName);
                    return property.Equals(fullPropertyName, StringComparison.OrdinalIgnoreCase);
                };
                yield return predicate;
            }
        }

        internal static string GetPropertyName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    // For Boxed Value Types 
                    var convertExpression = (UnaryExpression)expression;
                    return GetPropertyName(convertExpression.Operand);

                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;

                    var memberInfo = memberExpression.Member as PropertyInfo;
                    if (memberInfo != null)
                    {
                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                        {
                            // Chained expressions are not supported.
                            throw new InvalidOperationException(Resources.Chained_IncludePropertyExpression_NotSupported);
                        }

                        return memberInfo.Name;
                    }
                    else
                    {
                        // Fields are also not supported.
                        throw new InvalidOperationException(Resources.FormatInvalid_IncludePropertyExpression(
                            expression.NodeType));
                    }
                     
                default:
                    throw new InvalidOperationException(Resources.FormatInvalid_IncludePropertyExpression(
                        expression.NodeType));
            }
        }

        private static string CreatePropertyModelName(string prefix, string propertyName)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return propertyName ?? string.Empty;
            }
            else if (string.IsNullOrEmpty(propertyName))
            {
                return prefix ?? string.Empty;
            }
            else
            {
                return prefix + "." + propertyName;
            }
        }
    }
}