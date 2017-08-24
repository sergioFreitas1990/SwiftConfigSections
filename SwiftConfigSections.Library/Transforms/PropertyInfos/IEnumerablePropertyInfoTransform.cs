using IndependentUtils.Configuration;
using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools;
using IndependentUtils.Tools.Extensions;
using IndependentUtils.Tools.Objects;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms.PropertyInfos
{
    public class IEnumerablePropertyInfoTransform : IPropertyInfoTransform
    {
        public bool CanTransform(PropertyInfo property)
        {
            if (property == null)
            {
                return false;
            }

            var type = property.PropertyType;
            return type.IsInterface && type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                property.GetCustomAttribute<AutogeneratePropertyAttribute>() != null;
        }

        public IEnumerable<PropertyInfoModel> Transform(
            PropertyInfo property, ITransformFactory factory)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            // This is an IEnumerable, therefore, take its types
            var type = property.PropertyType;
            var genericType = type.GetGenericArguments().Single();

            // Transform this type.
            var createdClass = factory.Transform(genericType);
            // If the type was transformed, use the transformed,
            // instead the current
            var typeImplementation = createdClass != null ? 
                createdClass.FullName :
                genericType.FullName;

            var propertyOptions = property.GetCustomAttribute<AutogeneratePropertyAttribute>();

            var auxPropertyName = $"{property.Name}Aux";
            // Create the collection properties.
            // Property for the interface implementation
            yield return new PropertyInfoModel
            {
                 PropertyType = typeof(IEnumerable<>).MakeGenericFullName(genericType.FullName),
                 Name = property.Name,
                 GetterBody = auxPropertyName
            };

            // Real property
            var propertyType = typeof(ConfigurationElementCollection<>)
                .MakeGenericFullName(typeImplementation);
            yield return new PropertyInfoModel
            {
                Name = auxPropertyName,
                PropertyType = propertyType,
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
                    },
                    new AttributeModel
                    {
                        AttributeTypeFullName = typeof(ConfigurationCollectionAttribute).FullName,
                        ConstructorParameters = new List<object>
                        {
                            $"typeof({propertyType})"
                        },
                        AttributeValues = new List<SimpleTuple<string, object>>
                        {
                            SimpleTuple.Create<string, object>(
                                ReflectionUtils
                                    .GetPropertyInfo<ConfigurationCollectionAttribute>(
                                        t => t.AddItemName).Name, 
                                $"\"{propertyOptions.AddItemName}\"")
                        }
                    }
                },
                GetterBody = $"({propertyType})this[\"{propertyOptions.Name}\"]"
            };
        }
    }
}
