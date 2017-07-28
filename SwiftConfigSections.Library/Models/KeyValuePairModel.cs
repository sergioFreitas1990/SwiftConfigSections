using System;

namespace SwiftConfigSections.Library.Models
{
    [Serializable]
    public class KeyValuePairModel<TValue>
    {
        public string Key { get; set; }
        public TValue Value { get; set; }
    }
}
