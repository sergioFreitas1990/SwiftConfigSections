using System;
using System.Collections.Generic;

namespace SwiftConfigSections.Library.TemplateModels
{
    public class NamespaceModel : MarshalByRefObject
    {
        public string Namespace { get; set; }

        public List<ClassModel> Classes { get; set; }
    }
}