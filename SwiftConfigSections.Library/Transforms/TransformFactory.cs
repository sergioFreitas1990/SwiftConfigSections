using IndependentUtils.Configuration.Attributes;
using IndependentUtils.Tools.Extensions;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library.Transforms
{
    public class TransformFactory : ITransformFactory
    {
        private readonly IEnumerable<ITypeTransform> _typeTransforms;
        private readonly IEnumerable<IPropertyInfoTransform> _propertyTransforms;
        private readonly IList<ClassModel> _classes;
        private readonly IDictionary<Type, ClassModel> _processedTypes;

        public TransformFactory(IList<ClassModel> classes,
            IEnumerable<ITypeTransform> typeTransforms,
            IEnumerable<IPropertyInfoTransform> propertyTransforms)
        {
            _classes = classes;
            _propertyTransforms = propertyTransforms;
            _typeTransforms = typeTransforms;
            _processedTypes = new Dictionary<Type, ClassModel>();
        }

        public ClassModel Transform(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_processedTypes.TryGetValue(type, out ClassModel result))
            {
                // This type has already been processed.
                return result;
            }

            var parentClassModel = TransformType(type);
            if (parentClassModel == null)
            {
                // Type could not be resolved, move next.
                return null;
            }

            _processedTypes[type] = parentClassModel;
            var allProperties = type.GetProperties().Where(t => 
                t.GetCustomAttribute<AutogeneratePropertyAttribute>() 
                != null);

            foreach (var currProperty in allProperties)
            {
                var allPropertyModels = TransformPropertyInfo(currProperty, parentClassModel);
                parentClassModel.Properties.AddRange(allPropertyModels);
            }

            _classes.Add(parentClassModel);
            return parentClassModel;
        }

        private IEnumerable<PropertyInfoModel> TransformPropertyInfo(
            PropertyInfo property, ClassModel parentType)
        {
            var transformation = _propertyTransforms
                .SingleOrDefault(t => t.CanTransform(property));

            if (transformation == null)
            {
                // Cannot be resolved.
                return Enumerable.Empty<PropertyInfoModel>();
            }
            return transformation.Transform(property, this);
        }

        private ClassModel TransformType(Type type)
        {
            var transformation = _typeTransforms
                .SingleOrDefault(t => t.CanTransform(type));

            if (transformation == null)
            {
                // Cannot be resolved.
                return null;
            }

            // Type is processed.
            return transformation.Create(type);
        }
    }
}
