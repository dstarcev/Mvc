// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Mvc
{
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
}
