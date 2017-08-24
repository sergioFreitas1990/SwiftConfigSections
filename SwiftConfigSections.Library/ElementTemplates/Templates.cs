namespace SwiftConfigSections.Library.ElementTemplates
{
	public class Templates
	{
		private readonly Microsoft.VisualStudio.TextTemplating.ITextTemplatingSessionHost _host;
		private readonly Microsoft.VisualStudio.TextTemplating.VSHost.ITextTemplating _t4;

        public Templates(Microsoft.VisualStudio.TextTemplating.ITextTemplatingSessionHost host,
			Microsoft.VisualStudio.TextTemplating.VSHost.ITextTemplating t4)
        {
            _host = host;
			_t4 = t4;
        }
		public string CompileClassTemplate(SwiftConfigSections.Library.TemplateModels.ClassModel model)
		{
			var templateName = "ClassTemplate.t4";
			var template = ReadResourceFile(templateName);

			return Utils.TemplateCompiler.ProcessTemplate(
				_host, _t4, template, templateName, model);
		}

		public string CompileNamespaceTemplate(SwiftConfigSections.Library.TemplateModels.NamespaceModel model)
		{
			var templateName = "NamespaceTemplate.t4";
			var template = ReadResourceFile(templateName);

			return Utils.TemplateCompiler.ProcessTemplate(
				_host, _t4, template, templateName, model);
		}

		public string CompilePropertyTemplate(SwiftConfigSections.Library.TemplateModels.PropertyInfoModel model)
		{
			var templateName = "PropertyTemplate.t4";
			var template = ReadResourceFile(templateName);

			return Utils.TemplateCompiler.ProcessTemplate(
				_host, _t4, template, templateName, model);
		}

		private static string ReadResourceFile(string fileName)
        {
			var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var path = string.Format("SwiftConfigSections.Library.ElementTemplates.{0}", fileName);

            using (var stream = assembly.GetManifestResourceStream(path))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
	}
}
