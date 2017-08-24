using SwiftConfigSections.Library.TemplateModels;
using System;

namespace SwiftConfigSections.Library.Transforms
{
    public interface ITransformFactory
    {
        /// <summary>
        /// Transforms the type into a ClassModel. If the type fails to be
        /// resolved, then null is returned.
        /// </summary>
        ClassModel Transform(Type type);
    }
}
