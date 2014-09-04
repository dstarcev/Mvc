// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.Mvc
{
    public class Descriptor
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public ActionDescriptor ActionDescriptor { get; set; }

        public IEnumerable<UberBindingAttribute> BindingAttributes { get; set; }
    }

    public class PropertyDescriptor : Descriptor
    {
        public bool IsReadOnly { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }

    public class TypeDescriptor : Descriptor
    {
    }
}
