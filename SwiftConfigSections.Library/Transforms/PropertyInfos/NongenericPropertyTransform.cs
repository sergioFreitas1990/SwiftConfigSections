using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools.Objects;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms.PropertyInfos
{
    public class NongenericPropertyTransform : IPropertyInfoTransform
    {
        public bool CanTransform(PropertyInfo property)
        {
            if (property == null)
            {
                return false;
            }

            var type = property.PropertyType;
            return !type.IsGenericType &&
                property.GetCustomAttribute<AutogeneratePropertyAttribute>() != null;
        }

        public IEnumerable<PropertyInfoModel> Transform(PropertyInfo property, ITransformFactory factory)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            
            var type = property.PropertyType;
            // Transform this type.
            var createdClass = factory.Transform(type);
            // If the type was transformed, use the transformed,
            // instead the current
            var typeImplementation = createdClass != null ?
                createdClass.FullName :
                type.FullName;

            var propertyOptions = property.GetCustomAttribute<AutogeneratePropertyAttribute>();

            var auxPropertyName = $"{property.Name}Aux";
            // Create the properties.
            // Property for the interface implementation
            yield return new PropertyInfoModel
            {
                PropertyType = type.FullName,
                Name = property.Name,
                GetterBody = auxPropertyName
            };

            // Real property
            yield return new PropertyInfoModel
            {
                Name = auxPropertyName,
                PropertyType = typeImplementation,
                Attributes = new List<AttributeModel>
                {
                    new AttributeModel
                    {
                        AttributeTypeFullName = typeof(ConfigurationPropertyAttribute).FullName,
                        ConstructorParameters = new List<object>
                        {
                            $"\"{propertyOptions.Name}\""
                        },
                        AttributeValues = propertyOptions
                            .GetAllValues()
                            .Select(t => SimpleTuple.Create(t.Item1, t.Item2))
                            .ToList()
                    }
                },
                GetterBody = $"({typeImplementation})this[\"{propertyOptions.Name}\"]",
                SetterBody = $"this[\"{propertyOptions.Name}\"]"
            };
        }
    }
}
