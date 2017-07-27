using System;

namespace SwiftConfigSections.Library.Models
{
    [Serializable]
    public class TypeModel
    {
        public string TypeName { get; set; }

        public string InterfaceFullName { get; set; }

        public ValuePropertyModel[] ValueProperties { get; set; }

        public ListPropertyModel[] ListProperties { get; set; }

        public bool IsConfigSectionType { get; set; }

        public string GetKeyFunction { get; set; }
    }
}
