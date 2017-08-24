using SwiftConfigSections.Library.TemplateModels;
using System;

namespace SwiftConfigSections.Library.Transforms
{
    public interface ITypeTransform
    {
        bool CanTransform(Type type);

        ClassModel Create(Type type);
    }
}
