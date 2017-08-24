using SwiftConfigSections.Library.TemplateModels;
using System.Collections.Generic;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms
{
    /// <summary>
    /// Defines a transformation for a PropertyInfo that returns multiple
    /// PropertyInfoModel. The philosophy is that a single property can 
    /// originate multiple properties.
    /// </summary>
    public interface IPropertyInfoTransform
    {
        /// <summary>
        ///  Informs the caller that this implementation can or not
        /// transform this property info.
        /// </summary>
        bool CanTransform(PropertyInfo property);

        /// <summary>
        /// Creates a model from the specific property. If the CanTransform
        /// method isn't called before, there can be errors.
        /// </summary>
        /// <param name="factory">The factory in case a type needs to be resolved
        /// from the property level.</param>
        IEnumerable<PropertyInfoModel> Transform(
            PropertyInfo property, ITransformFactory factory);
    }
}
