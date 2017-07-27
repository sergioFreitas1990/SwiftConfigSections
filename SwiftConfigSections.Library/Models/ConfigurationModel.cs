using System;

namespace SwiftConfigSections.Library.Models
{
    [Serializable]
    public class ConfigurationModel
    {
        public string ConfigSectionName { get; set; }

        public string Namespace { get; set; }

        public TypeModel[] TypesToGenerate { get; set; }
    }
}
