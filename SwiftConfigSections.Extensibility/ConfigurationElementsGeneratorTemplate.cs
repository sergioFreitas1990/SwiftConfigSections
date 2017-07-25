using IndependentUtils.Configuration.Generation.Models;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System;
using System.Collections.Generic;
using System.IO;

namespace SwiftConfigSections.Extensibility
{
    public class ConfigurationElementsGeneratorTemplate
    {
        /// <sumary>
        /// The template that generates configuration sections.
        /// </sumary> 
        private const string TemplateName = "ConfigurationElementsGeneratorTemplate.t4";

        private readonly IDictionary<string, object> _parameters;

        public ConfigurationElementsGeneratorTemplate(string assemblyName,
            string namespaceName, ConfigurationModel model)
        {
            _parameters = new Dictionary<string, object>
            {
                { nameof(assemblyName), assemblyName },
                { nameof(namespaceName), namespaceName },
                { nameof(model), model }
            };
        }

        public string Create(ITextTemplating textTemplating)
        {
            return GenerateContentFromTemplate(textTemplating,
                TemplateName, _parameters);
        }

        /// <summary>
        /// Uses the template and generates the model.
        /// </summary>
        private static string GenerateContentFromTemplate(ITextTemplating t4, 
            string templateName, IDictionary<string, object> parameters)
        {
            var host = t4 as ITextTemplatingSessionHost;

            // Create a Session in which to pass parameters:  
            host.Session = host.CreateSession();

            // Add parameter values to the Session:  
            foreach (var curr in parameters)
            {
                host.Session[curr.Key] = curr.Value;
            }

            var callBacks = new EventTextTEmplateingCallback();
            callBacks.OnErrorCallback += (warning, message, line, column) =>
                throw new InvalidOperationException($"l{line},{column}: {message}");

            return t4.ProcessTemplate(templateName,
                File.ReadAllText(templateName), callBacks);
        }
    }
}
