using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templates;
using ERHMS.EpiInfo.Templates.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop
{
    public static class Utilities
    {
        private static readonly IDictionary<string, MethodInfo> Methods = typeof(Utilities)
            .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(method => method.Name != nameof(Main))
            .ToDictionary(method => method.Name, method => method);

        public static void Main(string[] args)
        {
            Log.Default.Debug($"Parsing arguments: {string.Join(", ", args)}");
            MethodInfo method = Methods[args[0]];
            object[] parameters = GetParameters(method, args).ToArray();
            Log.Default.Debug($"Invoking: {method.Name}");
            method.Invoke(null, GetParameters(method, args).ToArray());
        }

        private static IEnumerable<object> GetParameters(MethodInfo method, string[] args)
        {
            IList<ParameterInfo> parameters = method.GetParameters();
            if (args.Length - 1 != parameters.Count)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"The {method.Name} utility must be invoked with ");
                if (parameters.Count == 0)
                {
                    message.Append("no arguments");
                }
                else
                {
                    message.Append("the following arguments: ");
                    message.Append(string.Join(", ", parameters.Select(parameter => parameter.Name)));
                }
                message.Append(".");
                throw new ArgumentException(message.ToString());
            }
            for (int index = 0; index < parameters.Count; index++)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(parameters[index].ParameterType);
                yield return converter.ConvertFromString(args[index + 1]);
            }
        }

        private static void Report(string message)
        {
            MessageBox.Show(message, Resources.AppTitle);
        }

        public static void Help()
        {
            string methodNames = string.Join(", ", Methods.Keys.OrderBy(methodName => methodName));
            Report($"The following utilities are available: {methodNames}.");
        }

        public static void ResetSettings()
        {
            Settings.Default.Reset();
            Report("Settings have been reset.");
        }

        public static void CreateProjectTemplate(string projectPath, string templatePath)
        {
            if (File.Exists(templatePath))
            {
                throw new IOException("Template already exists.");
            }
            Project project = new Project(projectPath);
            ProjectTemplateCreator creator = new ProjectTemplateCreator(project);
            // TODO: Progress
            XTemplate xTemplate = creator.Create();
            using (Stream stream = File.Create(templatePath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
            Report("Template has been created.");
        }

        public static void InstantiateProjectTemplate(string templatePath, string projectPath)
        {
            if (File.Exists(projectPath))
            {
                throw new IOException("Project already exists.");
            }
            IDatabase database = new AccessDatabase(new OleDbConnectionStringBuilder
            {
                DataSource = Path.ChangeExtension(projectPath, AccessDatabase.FileExtension)
            });
            if (database.Exists())
            {
                throw new IOException("Database already exists.");
            }
            XTemplate xTemplate = new XTemplate(XDocument.Load(templatePath).Root);
            database.Create();
            Project project = ProjectExtensions.Create(new ProjectCreationInfo
            {
                Name = Path.GetFileNameWithoutExtension(projectPath),
                Location = Path.GetDirectoryName(projectPath),
                Database = database
            });
            project.Initialize();
            ProjectTemplateInstantiator instantiator = new ProjectTemplateInstantiator(xTemplate, project);
            // TODO: Progress
            instantiator.Instantiate();
            Report("Template has been instantiated.");
        }
    }
}
