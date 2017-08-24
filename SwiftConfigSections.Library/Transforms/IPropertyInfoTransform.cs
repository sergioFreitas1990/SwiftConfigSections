using SwiftConfigSections.Library.TemplateModels;
using System.Collections.Generic;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms
{
    public interface IPropertyInfoTransform
    {
        IEnumerable<PropertyInfoModel> Transform(
            PropertyInfo property, ITransformFactory factory);

        bool CanTransform(PropertyInfo property);
    }
}
