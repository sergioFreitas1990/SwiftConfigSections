using IndependentUtils.Tools.Extensions;
using SwiftConfigSections.Library.TemplateModels;
using SwiftConfigSections.Library.Transforms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library
{
    public static class NamespaceModelCreator
    {
        private static readonly IEnumerable<IPropertyInfoTransform> _propertyInfoTransforms = Assembly
            .GetExecutingAssembly()
            .GetAllImplementationsOf<IPropertyInfoTransform>();
        private static readonly IEnumerable<ITypeTransform> _typeTransforms = Assembly
            .GetExecutingAssembly()
            .GetAllImplementationsOf<ITypeTransform>();

        public static NamespaceModel CreateModel<T>()
        {
            return CreateModel(typeof(T));
        }

        public static NamespaceModel CreateModel(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var classes = new List<ClassModel>();
            var factory = new TransformFactory(classes,
                _typeTransforms, _propertyInfoTransforms);

            // Build all properties and types
            factory.Transform(type);
            return new NamespaceModel
            {
                Namespace = type.Namespace,
                Classes = classes
                    .OrderByDescending(t => 
                        t.BaseType == typeof(ConfigurationSection).FullName)
                    .ToList()
            };            
        }
    }
}