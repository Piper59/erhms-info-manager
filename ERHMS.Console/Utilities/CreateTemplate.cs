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
        private const string ViewNameWildcard = "*";

        public string ProjectPath { get; }
        public string ViewName { get; }
        public string TemplatePath { get; }

        public CreateTemplate(string projectPath, string viewName, string templatePath)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            TemplatePath = templatePath;
        }

        protected override void RunCore()
        {
            Project project = new Project(ProjectPath);
            TemplateCreator creator;
            if (ViewName == ViewNameWildcard)
            {
                creator = new ProjectTemplateCreator(project);
            }
            else
            {
                creator = new ViewTemplateCreator(project.Views[ViewName]);
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
