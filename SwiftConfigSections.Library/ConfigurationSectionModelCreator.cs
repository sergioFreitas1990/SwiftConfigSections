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
    public static class ConfigurationSectionModelCreator
    {
        private static readonly IEnumerable<IPropertyInfoTransform> _propertyInfoTransforms = Assembly
            .GetExecutingAssembly()
            .GetAllImplementationsOf<IPropertyInfoTransform>();
        private static readonly IEnumerable<ITypeTransform> _typeTransforms = Assembly
            .GetExecutingAssembly()
            .GetAllImplementationsOf<ITypeTransform>();

        /// <summary>
        /// Creates a namespace model which describes a namespace with types
        /// that implements the dependencies of the paramterized type.
        /// </summary>
        /// <typeparam name="T">The type to process.</typeparam>
        /// <returns>The constructed model.</returns>
        public static NamespaceModel CreateModel<T>()
        {
            return CreateModel(typeof(T));
        }

        /// <summary>
        /// Creates a namespace model which describes a namespace with types
        /// that implements the dependencies of the paramterized type.
        /// </summary>  
        /// <returns>The constructed model.</returns>
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