using Epi;
using ERHMS.Common;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.IO;
using System.Xml;

namespace ERHMS.Console.Utilities
{
    public class CreateTemplate : Utility
    {
        public string ProjectPath { get; }
        public string TemplatePath { get; }

        public CreateTemplate(string projectPath, string templatePath)
        {
            ProjectPath = projectPath;
            TemplatePath = templatePath;
        }

        protected override void RunCore()
        {
            if (File.Exists(TemplatePath))
            {
                throw new ArgumentException("Template already exists.");
            }
            Log.Default.Debug($"Opening project: {ProjectPath}");
            Project project = new Project(ProjectPath);
            ProjectTemplateCreator creator = new ProjectTemplateCreator(project)
            {
                Progress = new ProgressLogger()
            };
            XTemplate xTemplate = creator.Create();
            Log.Default.Debug($"Saving template: {TemplatePath}");
            using (Stream stream = File.Create(TemplatePath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
            Log.Default.Debug("Template has been created");
        }
    }
}
