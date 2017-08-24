using SwiftConfigSections.Library.TemplateModels;
using System;

namespace SwiftConfigSections.Library.Transforms
{
    /// <summary>
    /// Defines a factory that from a type can create a class model.
    /// </summary>
    public interface ITransformFactory
    {
        /// <summary>
        /// Transforms the type into a ClassModel. If the type fails to be
        /// resolved, then null is returned.
        /// </summary>
        ClassModel Transform(Type type);
    }
}
