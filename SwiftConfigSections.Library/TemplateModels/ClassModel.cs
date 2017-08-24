using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using SwiftConfigSections.Library.ElementTemplates;
using System.Collections.Generic;

namespace SwiftConfigSections.Library.TemplateModels
{
    public class ClassModel : MemberInfoModel
    {
        public List<PropertyInfoModel> Properties { get; set; }

        public List<string> Interfaces { get; set; }

        public string BaseType { get; set; }

        public string Namespace { get; set; }

        public string FullName
        {
            get
            {
                var namespacePrefix = string.IsNullOrWhiteSpace(Namespace) ?
                    string.Empty :
                    $"{Namespace}.";

                return $"{namespacePrefix}{Name ?? string.Empty}";
            }
        }

        public string ToString(ITextTemplatingSessionHost host, ITextTemplating t4)
        {
            return new Templates(host, t4).CompileClassTemplate(this);
        }
    }
}