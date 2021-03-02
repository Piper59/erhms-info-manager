using Epi;
using ERHMS.Common;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Linq;

namespace ERHMS.Console.Utilities
{
    public class InstantiateTemplate : IUtility
    {
        public string TemplatePath { get; }
        public string ProjectPath { get; }
        public string FormName { get; }
        public string PageName { get; }

        public InstantiateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        public InstantiateTemplate(string templatePath, string projectPath, string formName)
            : this(templatePath, projectPath)
        {
            FormName = formName;
        }

        public InstantiateTemplate(string templatePath, string projectPath, string formName, string pageName)
            : this(templatePath, projectPath, formName)
        {
            PageName = pageName;
        }

        public void Run()
        {
            XTemplate xTemplate = XTemplate.Load(TemplatePath);
            Project project = ProjectExtensions.Open(ProjectPath);
            TemplateInstantiator instantiator;
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    if (FormName != null)
                    {
                        throw new InvalidOperationException(
                            "Form name cannot be specified for a project-level template.");
                    }
                    instantiator = new ProjectTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.View:
                    if (PageName != null)
                    {
                        throw new InvalidOperationException("Page name cannot be specified for a form-level template.");
                    }
                    if (FormName != null)
                    {
                        XView xView = xTemplate.XProject.XViews.Single();
                        xView.Name = FormName;
                    }
                    instantiator = new ViewTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.Page:
                    if (FormName == null)
                    {
                        throw new InvalidOperationException("Form name must be specified for a page-level template.");
                    }
                    if (PageName != null)
                    {
                        XView xView = xTemplate.XProject.XViews.Single();
                        XPage xPage = xView.XPages.Single();
                        xPage.Name = PageName;
                    }
                    View view = project.Views[FormName];
                    instantiator = new PageTemplateInstantiator(xTemplate, view);
                    break;
                default:
                    throw new InvalidOperationException($"Template level '{xTemplate.Level}' is not supported.");
            }
            instantiator.Progress = Log.Progress;
            instantiator.Instantiate();
        }
    }
}
