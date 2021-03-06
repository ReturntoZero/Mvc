// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNet.Mvc.ModelBinding;
using Xunit;

namespace Microsoft.AspNet.Mvc.ApplicationModels
{
    public class PropertyModelTest
    {
        [Fact]
        public void CopyConstructor_CopiesAllProperties()
        {
            // Arrange
            var propertyModel = new PropertyModel(typeof(TestController).GetProperty("Property"),
                                               new List<object>() { new FromBodyAttribute() });

            propertyModel.Controller = new ControllerModel(typeof(TestController).GetTypeInfo(), new List<object>());
            propertyModel.BindingInfo = BindingInfo.GetBindingInfo(propertyModel.Attributes);
            propertyModel.PropertyName = "Property";

            // Act
            var propertyModel2 = new PropertyModel(propertyModel);

            // Assert
            foreach (var property in typeof(PropertyModel).GetProperties())
            {
                var value1 = property.GetValue(propertyModel);
                var value2 = property.GetValue(propertyModel2);

                if (typeof(IEnumerable<object>).IsAssignableFrom(property.PropertyType))
                {
                    Assert.Equal<object>((IEnumerable<object>)value1, (IEnumerable<object>)value2);

                    // Ensure non-default value
                    Assert.NotEmpty((IEnumerable<object>)value1);
                }
                else if (property.PropertyType.IsValueType ||
                    Nullable.GetUnderlyingType(property.PropertyType) != null)
                {
                    Assert.Equal(value1, value2);

                    // Ensure non-default value
                    Assert.NotEqual(value1, Activator.CreateInstance(property.PropertyType));
                }
                else
                {
                    Assert.Same(value1, value2);

                    // Ensure non-default value
                    Assert.NotNull(value1);
                }
            }
        }

        private class TestController
        {
            public string Property { get; set; }
        }
    }
}