using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Library.ElementTemplates.Utils
{
    public static class TemplateCompiler
    {
        public static string ProcessTemplate(ITextTemplatingSessionHost host,
            ITextTemplating t4, string template, string templateName, object model)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            if (t4 == null)
            {
                throw new ArgumentNullException("t4");
            }

            // Create a Session in which to pass parameters:  
            host.Session = host.CreateSession();

            // Add parameter values to the Session:  
            host.Session["Model"] = model;
            //host.Session["Session"] = host.Session;

            var callBacks = new EventTextTemplateingCallback();
            callBacks.OnErrorCallback += (warning, message, line, column) =>
            {
                const string prefix = ">>";
                throw new InvalidOperationException(
                    $"f:{templateName}, l:{line},{column} - {message}){Environment.NewLine}" +
                    $"{prefix}{template.Split(Environment.NewLine.ToCharArray())[line]}");
            };
            
            return t4.ProcessTemplate(templateName, template, callBacks);
        }
    }
}
