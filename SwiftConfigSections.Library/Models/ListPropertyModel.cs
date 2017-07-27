using System;

namespace SwiftConfigSections.Library.Models
{
    [Serializable]
    public class ListPropertyModel : ValuePropertyModel
    {
        public string MembersTypeFullName { get; set; }

        public string MembersTypeImplementationName { get; set; }

        public string AddItemName { get; set; }
    }
}
