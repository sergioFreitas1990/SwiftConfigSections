using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools.Extensions;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms.Types
{
    public class AutogenerateSectionTypeTransform : ITypeTransform
    {
        public bool CanTransform(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (!type.IsInterface)
            {
                return false;
            }
            var autogenerateAttribute = type
                .GetCustomAttribute<AutogenerateSectionAttribute>();
            if (autogenerateAttribute == null)
            {
                return false;
            }

            return true;
        }

        public ClassModel Create(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var autogenerateAttribute = type
                .GetCustomAttribute<AutogenerateSectionAttribute>();

            return new ClassModel
            {
                Name = type.Name.RemovePrefix("I"),
                Namespace = type.Namespace,
                BaseType = typeof(ConfigurationSection).FullName,
                Interfaces = type.FullName.ToEnumerable().ToList(),
                Properties = new List<PropertyInfoModel>(),
                Attributes = new List<AttributeModel>
                {
                    new AttributeModel
                    {
                        AttributeTypeFullName = typeof(SectionNameAttribute).FullName,
                        ConstructorParameters = new List<object>
                        {
                            $"\"{ autogenerateAttribute.SectionName}\""
                        }
                    }
                },
                Description = 
                    $"<section name=\"{autogenerateAttribute.SectionName}\" " +
                    $"type=\"{type.Namespace}.{type.Name.RemovePrefix("I")}, {type.Assembly.FullName}\" />"
                
            };
        }
    }
}
