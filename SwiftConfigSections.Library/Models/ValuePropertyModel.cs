using System;

namespace SwiftConfigSections.Library.Models
{
    [Serializable]
    public class ValuePropertyModel
    {
        public string PropertyName { get; set; }
        
        public string PropertyTypeName { get; set; }

        public string PropertyCastTypeName { get; set; }

        public string ConfigurationPropertyName { get; set; }

        public KeyValuePairModel<object>[] Options { get; set; }
    }
}
