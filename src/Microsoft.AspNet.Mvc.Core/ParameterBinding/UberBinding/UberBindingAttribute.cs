// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc.ModelBinding;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc
{
    // Class when it is declared on a type. 
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Class)]
    public abstract class UberBindingAttribute : Attribute
    {
        public abstract IUberBinding GetBinding(Descriptor descriptor);
    }
}
