// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace ModelBindingWebSite.Controllers
{
    public class BindAttributeController : Controller
    {
        private class ExcludeUserPropertiesAtParameter : DefaultModelPropertyFilterProvider<User>
        {
            public override string Prefix
            {
                get
                {
                    return "user";
                }
            }

            public override IEnumerable<Expression<Func<User, object>>> PropertyIncludeExpressions
            {
                get
                {
                    yield return m => m.RegisterationMonth;
                    yield return m => m.UserName;
                }
            }
        }

        public User EchoUser([Bind(typeof(ExcludeUserPropertiesAtParameter))] User user)
        {
            return user;
        }

        public Dictionary<string, string> 
           UpdateUserId_BlackListingAtEitherLevelDoesNotBind(
            [Bind(typeof(ExcludeLastName))] User2 param1,
            [Bind(Include = "Id")] User2 param2)
        {
            return new Dictionary<string, string>()
            {
                // LastName is excluded at parameter level.
                { "param1.LastName", param1.LastName },

                // Id is excluded because it is not explicitly included by the bind attribute at type level.
                { "param2.Id", param2.Id.ToString() },
            };
        }

        public Dictionary<string, string> UpdateFirstName_WhiteListingAtBothLevelBinds(
            [Bind(Include = "FirstName")] User2 param1)
        {
            return new Dictionary<string, string>()
            {
                // The since FirstName is included at both level it is bound.
                { "param1.FirstName", param1.FirstName },
            };
        }

        public Dictionary<string, string> UpdateIsAdmin_WhiteListingAtOnlyOneLevelDoesNotBind(
          [Bind(Include = "IsAdmin")] User2 param1)
        {
            return new Dictionary<string, string>()
            {
                // IsAdmin is not included because it is not explicitly included at type level.
                { "param1.IsAdmin", param1.IsAdmin.ToString() },

                // FirstName is not included because it is not explicitly included at parameter level.
                { "param1.FirstName", param1.FirstName },
            };
        }

        public string BindParameterUsingParameterPrefix([Bind(Prefix = "randomPrefix")] ParameterPrefix param)
        {
            return param.Value;
        }

        // This will use param to try to bind and not the value specified at TypePrefix.
        public string TypePrefixIsNeverUsed([Bind] TypePrefix param)
        {
            return param.Value;
        }
    }

    [Bind(Prefix = "TypePrefix")]
    public class TypePrefix
    {
        public string Value { get; set; }
    }

    public class ParameterPrefix
    {
        public string Value { get; set; }
    }

    [Bind(Include = nameof(FirstName) + "," + nameof(LastName))]
    public class User2
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class ExcludeLastName : IModelPropertyFilterProvider
    {
        public Func<ModelBindingContext, string, bool> PropertyFilter
        {
            get
            {
                return (context, propertyName) => !string.Equals("LastName", propertyName, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}