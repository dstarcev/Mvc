// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// Represents a type which provides a predicate which can be used to filter properties
    /// on a model.
    /// </summary>
    public interface IModelPropertyFilterProvider
    {

        /// <summary>
        /// Gets a filter which can be used to filter properties on a model at runtime.
        /// </summary>
        Func<ModelBindingContext, string, bool> PropertyFilter { get; }
    }
}
