using System;

namespace SwiftConfigSections.Library.TemplateModels
{
    /// <summary>
    /// A simple wrapper that allows for strings to be perceived as code on the
    /// ToStringValue extension method.
    /// </summary>
    public class ValueModel : MarshalByRefObject
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
