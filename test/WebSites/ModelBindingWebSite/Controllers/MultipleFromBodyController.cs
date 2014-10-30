// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc;

namespace ModelBindingWebSite.Controllers
{
    public class MultipleFromBodyController : Controller
    {
        public void FromBodyParametersThrows([FromBody] int id, [FromBody] string emp)
        {
        }

        public void FromBodyParameterAndPropertyThrows([FromBody] Person p, Customer customer)
        {
        }
    }
}