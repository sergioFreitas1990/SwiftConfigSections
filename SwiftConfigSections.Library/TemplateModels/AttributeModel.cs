using IndependentUtils.Tools.Extensions;
using IndependentUtils.Tools.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwiftConfigSections.Library.TemplateModels
{
    public class AttributeModel : MarshalByRefObject
    {
        public string AttributeTypeFullName { get; set; }

        public List<object> ConstructorParameters { get; set; }

        public List<SimpleTuple<string, object>> AttributeValues { get; set; }

        public override string ToString()
        {
            return $"[{AttributeTypeFullName.RemoveSuffix("Attribute")}{GeneratePerameters()}]";
        }

        /// <summary>
        /// Gets the consutructor and named parameters of an attribute formatted.
        /// Example:
        /// (1, "a-name", Value1 = "value1", Value2 = 2)
        /// </summary>
        private string GeneratePerameters()
        {
            var constructorParameters = ConstructorParameters
                .Select(t => t.ToValueString());

            // "Value1 = \"value1\"", "Value2 = 2"
            var attributeValues = AttributeValues
                .Select(t => $"{t.Item1} = {t.Item2.ToValueString()}");

            // Seperate the constructor and the named parameters with a comma.
            var values = string.Join(", ", constructorParameters.Concat(attributeValues));

            // If there are no values, then the attribute should be empty, otherwise,
            // Return the parameters within brackets
            return string.IsNullOrWhiteSpace(values) ? string.Empty : $"({values})";
        }
    }
}
