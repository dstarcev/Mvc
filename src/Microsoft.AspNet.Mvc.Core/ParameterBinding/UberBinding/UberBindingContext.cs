// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Microsoft.AspNet.Mvc
{
    public class UberBindingContext
    {
        // TODO: this cannot be moved in model binding as it is not aware of that.
        // Finally it should be moved to model binding. 
        public ActionContext ActionContext { get; set; }

        public HttpContext HttpContext { get; set; }

        public ModelStateDictionary ModelState { get; set; }

        public IModelMetadataProvider MetadataProvider { get; internal set; }

        public object Model { get; set; }
        public IModelBinder ModelBinder { get; internal set; }
        public ModelMetadata ModelMetadata { get; internal set; }
        public string ModelName { get; internal set; }
        public IModelValidatorProvider ValidatorProvider { get; internal set; }
        public IValueProvider ValueProvider { get; internal set; }
    }
}
