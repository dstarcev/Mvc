// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.Routing;

namespace Microsoft.AspNet.Mvc.ReflectedModelBuilder
{
    public class ReflectedControllerModel
    {
        public ReflectedControllerModel([NotNull] TypeInfo controllerType)
        {
            ControllerType = controllerType;

            Actions = new List<ReflectedActionModel>();

            // CoreCLR returns IEnumerable<Attribute> from GetCustomAttributes - the OfType<object>
            // is needed to so that the result of ToList() is List<object>
            Attributes = ControllerType.GetCustomAttributes(inherit: true).OfType<object>().ToList();

            Filters = Attributes.OfType<IFilter>().ToList();
            RouteConstraints = Attributes.OfType<RouteConstraintAttribute>().ToList();

            var routeTemplateAttribute = Attributes.OfType<IRouteTemplateProvider>().FirstOrDefault();
            if (routeTemplateAttribute != null)
            {
                AttributeRouteModel = new ReflectedAttributeRouteModel(routeTemplateAttribute);
            }

            ControllerName = controllerType.Name.EndsWith("Controller", StringComparison.Ordinal)
                        ? controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length)
                        : controllerType.Name;
            var properties = controllerType.GetRuntimeProperties().Where(
                    prop => prop.GetIndexParameters().Length == 0 &&
                    prop.GetMethod != null &&
                    prop.GetMethod.IsPublic &&
                    !prop.GetMethod.IsStatic);

            Properties = properties.Select(property => {
                var attributes = property.GetCustomAttributes(inherit: false).OfType<object>().ToList();
                return new ReflectedPropertyModel
                {
                    Name = property.Name,
                    PropertyType = property.PropertyType,
                    Attributes = attributes
                };
            }).ToList();
        }

        public List<ReflectedActionModel> Actions { get; private set; }

        public List<object> Attributes { get; private set; }

        public string ControllerName { get; set; }

        public TypeInfo ControllerType { get; private set; }

        public List<ReflectedPropertyModel> Properties { get; private set; }

        public List<IFilter> Filters { get; private set; }

        public List<RouteConstraintAttribute> RouteConstraints { get; private set; }

        public ReflectedAttributeRouteModel AttributeRouteModel { get; set; }
    }

    public class ReflectedPropertyModel
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public List<object> Attributes { get; set; }
    }
}