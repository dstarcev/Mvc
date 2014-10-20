// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Mvc;

namespace ModelBindingWebSite
{
    public class ShoppingCart
    {
        public Address ShippingAddress { get; set; }

        public Address BillingAddress { get; set; }
    }


    public class ShoppingController : Controller
    {
        //public bool Checkout(int userId)
        //{
        //    var shoppingCart = GetShoppingCartFromUserId(userId);

        //    TryUpdateModelAsync(shoppingCart, string.Empty, null, (container, propertyType, propertyName) => {

        //        if (propertyType == typeof(Address))
        //        {

        //        }
        //    });

        //    retyn t
        //}

        private ShoppingCart GetShoppingCartFromUserId(int userId)
        {
            return new ShoppingCart();
        }
    }
}