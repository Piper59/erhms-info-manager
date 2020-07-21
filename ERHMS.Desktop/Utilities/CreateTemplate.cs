using Epi;
using ERHMS.EpiInfo.Templates;
using ERHMS.EpiInfo.Templates.Xml;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace ERHMS.Desktop.Utilities
{
    public class CreateTemplate : Utility
    {
        protected override bool LongRunning => true;
        public string ProjectPath { get; }
        public string TemplatePath { get; }

        public CreateTemplate(string projectPath, string templatePath)
        {
            ProjectPath = projectPath;
            TemplatePath = templatePath;
        }

        protected override async Task<string> RunCoreAsync()
        {
            await Task.Run(() =>
            {
                if (File.Exists(TemplatePath))
                {
                    throw new ArgumentException("Template already exists.");
                }
                Project project = new Project(ProjectPath);
                ProjectTemplateCreator creator = new ProjectTemplateCreator(project)
                {
                    Progress = Progress
                };
                XTemplate xTemplate = creator.Create();
                using (Stream stream = File.Create(TemplatePath))
                using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
                {
                    xTemplate.Save(writer);
                }
            });
            return "Template has been created.";
        }
    }
}
