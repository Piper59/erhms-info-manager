using Epi;
using ERHMS.Common;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;
using System.Xml;

namespace ERHMS.Console.Utilities
{
    public class CreateTemplate : Utility
    {
        private const string FormNameWildcard = "*";

        public string ProjectPath { get; }
        public string FormName { get; }
        public string TemplatePath { get; }

        public CreateTemplate(string projectPath, string formName, string templatePath)
        {
            ProjectPath = projectPath;
            FormName = formName;
            TemplatePath = templatePath;
        }

        protected override void RunCore()
        {
            Project project = new Project(ProjectPath);
            TemplateCreator creator;
            if (FormName == FormNameWildcard)
            {
                creator = new ProjectTemplateCreator(project);
            }
            else
            {
                creator = new ViewTemplateCreator(project.Views[FormName]);
            }
            creator.Progress = new ProgressLogger();
            XTemplate xTemplate = creator.Create();
            using (Stream stream = File.Create(TemplatePath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
            Log.Default.Debug("Template has been created");
        }
    }
}
