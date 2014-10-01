// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.Mvc
{
    public class ParameterBindingInfo
    {
        public ParameterBindingInfo(string prefix, Type parameterType, object bindingMetadata = null)
        {
            Prefix = prefix;
            ParameterType = parameterType;
            BindingMetadata = bindingMetadata;
        }

        public string Prefix { get; private set; }

        public Type ParameterType { get; private set; }

        public object BindingMetadata { get; set; }
    }
}
