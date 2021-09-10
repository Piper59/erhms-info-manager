using Epi;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.Console.Utilities
{
    public class CreateTemplate : Utility
    {
        public string TemplatePath { get; }
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string PageName { get; }

        public CreateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        public CreateTemplate(string templatePath, string projectPath, string viewName)
            : this(templatePath, projectPath)
        {
            ViewName = viewName;
        }

        public CreateTemplate(string templatePath, string projectPath, string viewName, string pageName)
            : this(templatePath, projectPath, viewName)
        {
            PageName = pageName;
        }

        private TemplateCreator GetCreator()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            if (ViewName == null)
            {
                return new ProjectTemplateCreator(project);
            }
            else
            {
                View view = project.Views[ViewName];
                if (PageName == null)
                {
                    return new ViewTemplateCreator(view);
                }
                else
                {
                    Page page = view.GetPageByName(PageName);
                    return new PageTemplateCreator(page);
                }
            }
        }

        public override void Run()
        {
            TemplateCreator creator = GetCreator();
            creator.Progress = Log.Progress;
            XTemplate xTemplate = creator.Create();
            xTemplate.Save(TemplatePath);
        }
    }
}
