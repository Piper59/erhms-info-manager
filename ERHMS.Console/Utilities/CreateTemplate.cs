using Epi;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;
using System.Xml;

namespace ERHMS.Console.Utilities
{
    public class CreateTemplate : Utility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string TemplatePath { get; }

        public CreateTemplate(string projectPath, string templatePath)
        {
            ProjectPath = projectPath;
            TemplatePath = templatePath;
        }

        public CreateTemplate(string projectPath, string viewName, string templatePath)
            : this(projectPath, templatePath)
        {
            ViewName = viewName;
        }

        protected override void RunCore()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            TemplateCreator creator;
            if (ViewName == null)
            {
                creator = new ProjectTemplateCreator(project);
            }
            else
            {
                creator = new ViewTemplateCreator(project.Views[ViewName]);
            }
            creator.Progress = new LoggingProgress();
            XTemplate xTemplate = creator.Create();
            using (Stream stream = File.Create(TemplatePath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
        }
    }
}
