using SwiftConfigSections.Library.Models;
using System;

namespace SwiftConfigSections.Extensibility
{
    [Serializable]
    public class GenerateContentFromTemplateParameters
    {
        public string AssemblyName { get; set; }
        public string NamespaceName { get; set; }
        public ConfigurationModel Model { get; set; }
    }
}
