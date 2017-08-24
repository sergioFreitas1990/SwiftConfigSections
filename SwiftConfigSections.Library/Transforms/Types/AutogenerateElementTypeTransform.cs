using IndependentUtils.Configuration;
using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools;
using IndependentUtils.Tools.Extensions;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms.Types
{
    public class AutogenerateElementTypeTransform : ITypeTransform
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
                .GetCustomAttribute<AutogenerateElementAttribute>();
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
                .GetCustomAttribute<AutogenerateElementAttribute>();

            var className = type.Name.RemovePrefix("I");
            var keyFunc = new PropertyInfoModel
            {
                Name = $"{className}KeyFunc",
                Description =
                    $"A simple function that generates the {className}'s key from the provided " +
                    "lambda, so that it can be consumed by the other property.",
                GetterBody = autogenerateAttribute.LambdaString,
                PropertyType = typeof(Func<,>)
                    .MakeGenericFullName(className, typeof(object).FullName)
            };
            var keyProperty = new PropertyInfoModel
            {
                Name = ReflectionUtils.GetPropertyInfo<IKeyd>(t => t.Key).Name,
                PropertyType = typeof(object).FullName,
                GetterBody = $"{keyFunc.Name}(this)"
            };

            var classModel = new ClassModel
            {
                Name = className,
                Namespace = string.Empty,
                BaseType = typeof(ConfigurationElement).FullName,
                Interfaces = new List<string>
                {
                    typeof(IKeyd).FullName,
                    type.FullName
                },
                Properties = new List<PropertyInfoModel>
                {
                    keyFunc,
                    keyProperty
                }
            };
            return classModel;
        }
    }
}
