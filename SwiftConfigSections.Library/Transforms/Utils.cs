using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools.Objects;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SwiftConfigSections.Library.Transforms
{
    public static class Utils
    {
        public static AttributeModel ToModel(this AutogeneratePropertyAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var allPropertiesOfAttribute = attribute
                .GetAllValues()
                .Select(t => SimpleTuple.Create(t.Item1, t.Item2))
                .ToList();

            return new AttributeModel
            {
                AttributeTypeFullName = typeof(ConfigurationPermissionAttribute).FullName,
                ConstructorParameters = new List<object>
                {
                    attribute.Name
                },
                AttributeValues = allPropertiesOfAttribute
            };
        }
    }
}
