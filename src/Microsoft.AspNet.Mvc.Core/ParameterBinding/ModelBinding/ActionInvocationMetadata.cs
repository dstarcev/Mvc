// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace Microsoft.AspNet.Mvc
{
    // Describes a complex model, but uses a collection rather than individual properties as the data store.
    public class ActionInvocationMetadata
    {
        //public ActionInvocationMetadata([NotNull] ModelMetadata modelMetadata, 
        //                                [NotNull] IEnumerable<UberBindingModelMetadata> propertyMetadata,
        //                                [NotNull] IEnumerable<UberBindingModelMetadata> parameterMetadata)
        //{
        //    ModelMetadata = modelMetadata;
        //    PropertyMetadata = propertyMetadata.ToList();
        //    ParameterMetadata = parameterMetadata.ToList();
        //    Results = new Dictionary<ModelMetadata, ComplexModelDtoResult>();
        //}

        //public ModelMetadata ModelMetadata { get; private set; }

        //public IReadOnlyList<UberBindingModelMetadata> PropertyMetadata { get; private set; }

        //public IReadOnlyList<UberBindingModelMetadata> ParameterMetadata { get; private set; }

        //// Contains entries corresponding to each property against which binding was
        //// attempted. If binding failed, the entry's value will be null. If binding
        //// was never attempted, this dictionary will not contain a corresponding
        //// entry.
        //public IDictionary<ModelMetadata, ComplexModelDtoResult> Results { get; private set; }

        public IList<BindingInfo> Parameters { get; set; }

        public IList<BindingInfo> Properties { get; set; }
    }

    public class BindingInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public IUberBinding Binding { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
        public ParameterInfo ParameterInfo { get; set; }
    }
}
