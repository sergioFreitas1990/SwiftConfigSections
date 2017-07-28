using IndependentUtils.Configuration.Attributes;
using SwiftConfigSections.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library
{
    public static class SectionGenerator
    {
        public static ConfigurationModel Generate(Type inputType)
        {
            if (inputType == null)
            {
                throw new ArgumentNullException(nameof(inputType));
            }
            if (!inputType.IsInterface)
            {
                throw new InvalidOperationException($"Type {inputType} is not an interface!");
            }
            var configSectionAttribute = inputType.GetCustomAttribute<AutogenerateSectionAttribute>();
            if (configSectionAttribute == null)
            {
                throw new InvalidOperationException($"Interface type {inputType} " +
                    $"does not contain {typeof(AutogenerateSectionAttribute).Name} attribute!");
            }

            return new ConfigurationModel
            {
                ConfigSectionName = configSectionAttribute.SectionName,
                TypesToGenerate = GenerateTypeModels(inputType).ToArray(),
                Namespace = inputType.Namespace
            };
        }

        /// <summary>
        /// Recursively generates all the type information required to build the config
        /// section and configuration elements.
        /// </summary>
        private static IEnumerable<TypeModel> GenerateTypeModels(Type rootType)
        {
            var list = new List<TypeModel>();
            GenerateTypeModels(rootType, list, true);
            return list;
        }

        /// <summary>
        /// Adds to the list the newly generated type. It will also check for nested types within
        /// the elements.
        /// </summary>
        /// <param name="isConfigSectionType">Whether or not the current element is the
        /// config section type.</param>
        private static void GenerateTypeModels(Type rootType, IList<TypeModel> types,
            bool isConfigSectionType = false)
        {
            if (types.Select(t => t.InterfaceFullName).Contains(rootType.FullName))
            {
                // There is no need to add the same type again.
                return;
            }

            var getKeyFunc = !isConfigSectionType ?
                rootType.GetCustomAttribute<AutogenerateElementAttribute>().LambdaString :
                null;

            var listProperties = new List<ListPropertyModel>();
            var valueProperties = new List<ValuePropertyModel>();

            foreach (var currProperty in rootType.GetProperties())
            {
                if (currProperty.SetMethod != null)
                {
                    throw new InvalidOperationException(
                        "There is no support for Setters on the interface." +
                        $"Remove the setter from {rootType.Name}.{currProperty.Name}.");
                }

                ValuePropertyModel currModel;
                var currPropertyAttribute = currProperty
                    .GetCustomAttribute<AutogeneratePropertyAttribute>();
                if (currPropertyAttribute == null)
                {
                    throw new InvalidOperationException("Cannot implement the interface, " +
                        $"the property {currProperty.Name} does not contain " +
                        $"{typeof(AutogeneratePropertyAttribute).Name} attribute!");
                }

                var currPropertyType = currProperty.PropertyType;
                if (currPropertyType.IsGenericType)
                {
                    if (currPropertyType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                    {
                        // Ups! This is unsupported!
                        throw new InvalidOperationException(
                            "There is only support for IEnumerable<T> types: Type " +
                            $"{currPropertyType.GetGenericTypeDefinition().Name} " +
                            "is not an IEnumerable<T>.");
                    }

                    // This is an ienumerable, therefore, take its types
                    var genericType = currPropertyType.GetGenericArguments().Single();
                    GenerateTypeModels(genericType, types);

                    var currPropertyModel = new ListPropertyModel
                    {
                        MembersTypeFullName = genericType.FullName,
                        MembersTypeImplementationName = RemoveIFromInterface(genericType.Name),
                        AddItemName = currPropertyAttribute.AddItemName
                    };
                    currModel = currPropertyModel;
                    currModel.PropertyCastTypeName = currProperty.PropertyType.FullName;
                    listProperties.Add(currPropertyModel);
                }
                else
                {
                    currModel = new ValuePropertyModel();
                    if (currPropertyType.IsInterface)
                    {
                        GenerateTypeModels(currPropertyType, types);
                        currModel.PropertyCastTypeName = RemoveIFromInterface(currProperty.PropertyType.Name);
                    }
                    else
                    {
                        currModel.PropertyCastTypeName = currProperty.PropertyType.FullName;
                    }                    
                    valueProperties.Add(currModel);
                }

                currModel.PropertyTypeName = currProperty.PropertyType.FullName;
                currModel.ConfigurationPropertyName = currPropertyAttribute.Name;
                currModel.PropertyName = currProperty.Name;
                currModel.Options = currPropertyAttribute.AdditionalSettings.Select(t => 
                    new KeyValuePairModel<object>
                    {
                        Key = t.Key,
                        Value = t.Value
                    }).ToArray();
            }

            types.Add(new TypeModel
            {
                IsConfigSectionType = isConfigSectionType,
                ValueProperties = valueProperties.ToArray(),
                ListProperties = listProperties.ToArray(),
                TypeName = RemoveIFromInterface(rootType.Name),
                InterfaceFullName = $"{rootType.Namespace}.{rootType.Name}",
                GetKeyFunction = getKeyFunc
            });
        }

        private static string RemoveIFromInterface(string typeName)
        {
            if (typeName.StartsWith("I"))
            {
                return typeName.Substring(1);
            }
            return typeName;
        }
    }
}
