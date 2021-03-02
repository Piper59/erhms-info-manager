using Epi;
using ERHMS.Common;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;
using System.Linq;

namespace ERHMS.Console.Utilities
{
    public class CreateTemplate : IUtility
    {
        public string TemplatePath { get; }
        public string ProjectPath { get; }
        public string FormName { get; }
        public string PageName { get; }

        public CreateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        public CreateTemplate(string templatePath, string projectPath, string formName)
            : this(templatePath, projectPath)
        {
            FormName = formName;
        }

        public CreateTemplate(string templatePath, string projectPath, string formName, string pageName)
            : this(templatePath, projectPath, formName)
        {
            PageName = pageName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            TemplateCreator creator;
            if (FormName == null)
            {
                creator = new ProjectTemplateCreator(project);
            }
            else
            {
                View view = project.Views[FormName];
                if (PageName == null)
                {
                    creator = new ViewTemplateCreator(view);
                }
                else
                {
                    Page page = view.Pages.Single(_page => NameComparer.Default.Equals(_page.Name, PageName));
                    creator = new PageTemplateCreator(page);
                }
            }
            creator.Progress = Log.Progress;
            XTemplate xTemplate = creator.Create();
            Directory.CreateDirectory(Path.GetDirectoryName(TemplatePath));
            xTemplate.Save(TemplatePath);
        }
    }
}
