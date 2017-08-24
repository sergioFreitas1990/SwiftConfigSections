using System;
using System.Collections.Generic;

namespace SwiftConfigSections.Library.TemplateModels
{
    public abstract class MemberInfoModel : MarshalByRefObject
    {
        public List<AttributeModel> Attributes { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
