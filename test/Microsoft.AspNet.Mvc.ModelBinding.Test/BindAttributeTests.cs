// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Xunit;

namespace Microsoft.AspNet.Mvc.ModelBinding.Test
{
    public class BindAttributeTest
    {
        [Fact]
        public void PrefixPropertyDefaultsToNull()
        {
            // Arrange
            BindAttribute attr = new BindAttribute();

            // Act & assert
            Assert.Null(attr.Prefix);
        }

        [Fact]
        public void Constructor_Throws_IfTypeDoesNotImplement_IModelPropertyFilterProvider()
        {
            // Arrange
            // Act & assert
            var exception = Assert.Throws<ArgumentException>(() => new BindAttribute(typeof(UnrelatedType)));
            Assert.Equal(string.Format("The type '{0}' must derive from '{1}'.\r\nParameter name: filterProviderType",
                                       typeof(UnrelatedType).FullName,
                                       typeof(IModelPropertyFilterProvider).FullName),
                        exception.Message);
        }

        [Theory]
        [InlineData(typeof(DerivedType))]
        [InlineData(typeof(BaseType))]
        public void Constructor_SetsThe_PropertyFilterProviderType_ForValidTypes(Type type)
        {
            // Arrange
            BindAttribute attr = new BindAttribute(type);

            // Act & assert
            Assert.Equal(type, attr.PropertyFilterProviderType);
        }
       
        private class BaseType : IModelPropertyFilterProvider
        {
            public Func<ModelBindingContext, string, bool> PropertyFilter
            {
                get
                {
                    throw new NotImplementedException();
                }
            }
        }

        private class DerivedType : BaseType
        {
        }

        private class UnrelatedType
        {
        }
    }
}
