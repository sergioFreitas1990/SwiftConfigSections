using SwiftConfigSections.Library.TemplateModels;
using System;

namespace SwiftConfigSections.Library.Transforms
{
    /// <summary>
    /// Defines a transformation for a Type that returns a ClassModel.
    /// </summary>
    public interface ITypeTransform
    {
        /// <summary>
        /// Informs the caller that this implementation can or not
        /// transform this type.
        /// </summary>
        bool CanTransform(Type type);

        /// <summary>
        /// Creates a model from the specific type. If the CanTransform
        /// method isn't called before, there can be errors.
        /// </summary>
        ClassModel Create(Type type);
    }
}
