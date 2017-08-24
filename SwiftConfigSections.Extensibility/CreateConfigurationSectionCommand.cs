//------------------------------------------------------------------------------
// <copyright file="CreateConfigurationSectionCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using EnvDTE;
using EnvDTE80;
using IndependentUtils.Tools.Extensions;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using SwiftConfigSections.Library;
using SwiftConfigSections.Library.ElementTemplates;
using SwiftConfigSections.Library.TemplateModels;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SwiftConfigSections.Extensibility
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CreateConfigurationSectionCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("15e2f49a-9e6b-4858-bd20-e771a44b7c13");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateConfigurationSectionCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CreateConfigurationSectionCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException("package");

            var commandService = CommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                menuItem.BeforeQueryStatus += ShowItemIfSelectedFileIsValid;
                commandService.AddCommand(menuItem);
            }
        }

        private void ShowItemIfSelectedFileIsValid(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;

            var isValid = ValidateCommandInvocation(Dte,
                count => menuCommand.Visible = false,
                fileName => menuCommand.Visible = false) != null;

            if (isValid)
            {
                menuCommand.Visible = true;
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CreateConfigurationSectionCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        private ITextTemplating T4 => ServiceProvider
            .GetService<STextTemplating, ITextTemplating>();

        private DTE2 Dte => ServiceProvider
            .GetService<DTE, DTE2>();

        private OleMenuCommandService CommandService => ServiceProvider
            .GetService<IMenuCommandService, OleMenuCommandService>();

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CreateConfigurationSectionCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = Dte;

            var selectedFile = ValidateCommandInvocation(dte,
                count => throw new InvalidOperationException(
                    "The command can only be applied to a single item."),
                fileName => throw new InvalidOperationException(
                    $"File {fileName} is not a .cs"));

            var selectedFileName = selectedFile.Name;
            var className = selectedFileName.Substring(0,
                selectedFileName.LastIndexOf('.'));

            // Gets the specific type from the project.
            var project = GetSelectedProject(dte);
            var projectAssemblyFile = GetAssemblyFileFromProject(project);

            if (projectAssemblyFile == null)
            {
                throw new InvalidOperationException(
                    $"Could not find a .dll associated with {project.Name}. " +
                    $"Make sure that the project is compiled.");
            }

            var arguments = new GenerateTemplateArguments
            {
                InterfaceName = className,
                SelectedFileName = selectedFileName
            };
            var namespaceModel = AppDomainRunner.Run(
                projectAssemblyFile.FullName, 
                arguments,
                GenerateModel);

            var destinationDirecotry = selectedFile.Directory;
            var fileFullName = Path.Combine(
                destinationDirecotry.FullName, 
                $"{className.RemovePrefix("I")}.cs");
            
            var templateGenerator = new Templates(T4 as ITextTemplatingSessionHost, T4);
            var fileContents = templateGenerator.CompileNamespaceTemplate(namespaceModel);
            File.WriteAllText(fileFullName, fileContents);

            // Add the project item.
            project.ProjectItems.AddFromFile(fileFullName);
        }

        [Serializable]
        private class GenerateTemplateArguments
        {
            public string InterfaceName { get; set; }
            public string SelectedFileName { get; set; }
        }

        private static NamespaceModel GenerateModel(
            Assembly assembly, GenerateTemplateArguments arguments)
        {
            var interfaceType = assembly
                .GetTypes()
                .FirstOrDefault(t => t.Name == arguments.InterfaceName);

            if (interfaceType == null)
            {
                throw new InvalidOperationException(
                    $"Make sure that the file {arguments.SelectedFileName} is compiled and has " +
                    $"an interface named {arguments.InterfaceName}.");
            }

            // Generate the file from the T4 Text Template
            var model = ConfigurationSectionModelCreator.CreateModel(interfaceType);
            return model;
        }

        /// <summary>
        /// Loads the assembly associated to the specific project if the project
        /// has been compiled.
        /// </summary>
        /// <returns>
        /// The loaded assembly or null if not found (probably not compiled).
        /// </returns>
        public static FileInfo GetAssemblyFileFromProject(Project project)
        {
            var projectNameDll = $"{project.Name}.dll";
            var dllFile = Directory
                .GetFiles(new FileInfo(project.FullName).DirectoryName,
                    projectNameDll,
                    SearchOption.AllDirectories)
                .FirstOrDefault();

            if (dllFile == null)
            {
                return null;
            }

            return new FileInfo(dllFile);
        }

        private FileInfo ValidateCommandInvocation(DTE2 dte,
            Action<int> invalidNumberOfItems,
            Action<string> invalidFileExtensions)
        {
            // Get the selected item
            var items = ((Array)dte.ToolWindows.SolutionExplorer.SelectedItems)
                .Cast<UIHierarchyItem>()
                .Select(t => (ProjectItem)t.Object)
                .ToList();

            var itemsCount = items.Count();
            if (itemsCount > 1)
            {
                invalidNumberOfItems(itemsCount);
                return null;
            }

            var fileInfo = new FileInfo(items[0].FileNames[1]);
            var fileName = fileInfo.Name;
            if (!fileName.EndsWith(".cs"))
            {
                invalidFileExtensions(fileName);
                return null;
            }

            // Validations passed!
            return fileInfo;
        }

        public static Project GetSelectedProject(DTE2 dte)
        {
            if (dte == null)
            {
                throw new ArgumentNullException(nameof(dte));
            }
            return ((Array)dte.ActiveSolutionProjects).Cast<Project>().Single();
        }
    }
}
