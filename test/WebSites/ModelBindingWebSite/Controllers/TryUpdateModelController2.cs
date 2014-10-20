// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace ModelBindingWebSite.Controllers
{
    public class TryUpdateModel2Controller : Controller
    {
        public async Task<User> GetUserAsync_IncludeAllByDefault(int id)
        {
            var user = GetUser(id);

            await TryUpdateModelAsync<User>(user, string.Empty);
            return user;
        }

        public async Task<User> GetUserAsync_ExcludeSpecificProperties(int id)
        {
            var user = GetUser(id);
            await TryUpdateModelAsync(user,
                                      prefix: string.Empty,
                                      predicate: (context, modelName) => !string.Equals(modelName, nameof(User.Id),StringComparison.Ordinal) &&
                                                              !string.Equals(modelName, nameof(User.Key), StringComparison.Ordinal)
                                      );

            return user;
        }

        public async Task<User> GetUserAsync_IncludeSpecificProperties(int id)
        {
            var user = GetUser(id);
            await TryUpdateModelAsync(user,
                                      prefix: string.Empty,
                                      includeExpressions: model => model.RegisterationMonth);

            return user;
        }

        public async Task<bool> TryUpdateModelFails(int id)
        {
            var user = GetUser(id);
            return await TryUpdateModelAsync(user,
                                             prefix: string.Empty,
                                             valueProvider: new CustomValueProvider());
        }

        public async Task<User> GetUserAsync_IncludeAndExcludeListNull(int id)
        {
            var user = GetUser(id);
            await TryUpdateModelAsync(user);

            return user;
        }

        public async Task<User> GetUserAsync_UpdateSubProperty(int id)
        {
            var user = GetUser(id);

            // This means that if Address is created, only include state inside it ( i.e exclude everything else).
            await TryUpdateModelAsync(user, string.Empty, includeExpressions: model => model.Address.Street);

            return user;
        }

        public async Task<User> GetUserAsync_UpdateSubArrayProperty(int id)
        {
            var user = GetUser(id);

            // This means that if Address.Country.Cities is created, only include city names for all cities.
            await TryUpdateModelAsync(user, string.Empty, model => model.Address.Country.Cities[0].CityName, model => model.Address.Country.StateCodes);

            return user;
        }

        public async Task<User> GetUserAsync_IncludesAllSubProperties(int id)
        {
            var user = GetUser(id);

            // Include everything under model.Address.Country.
            await TryUpdateModelAsync(user, string.Empty, includeExpressions: model => model.Address.Country);

            return user;
        }

        private User GetUser(int id)
        {
            return new User
            {
                UserName = "User_" + id,
                Id = id,
                Key = id + 20,
            };
        }

        public class CustomValueProvider : IValueProvider
        {
            public Task<bool> ContainsPrefixAsync(string prefix)
            {
                return Task.FromResult(false);
            }

            public Task<ValueProviderResult> GetValueAsync(string key)
            {
                return Task.FromResult<ValueProviderResult>(null);
            }
        }
    }
}