using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using SwiftConfigSections.Library.ElementTemplates;

namespace SwiftConfigSections.Library.TemplateModels
{
    public class PropertyInfoModel : MemberInfoModel
    {
        public string PropertyType { get; set; }

        public string GetterBody { get; set; }

        public string SetterBody { get; set; }

        public string ToString(ITextTemplatingSessionHost host, ITextTemplating t4)
        {
            return new Templates(host, t4).CompilePropertyTemplate(this);
        }
    }
}
