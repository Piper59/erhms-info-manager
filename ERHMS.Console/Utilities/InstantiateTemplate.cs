using Epi;
using ERHMS.Common.Logging;
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
        public string ViewName { get; }
        public string PageName { get; }

        public InstantiateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        public InstantiateTemplate(string templatePath, string projectPath, string viewName)
            : this(templatePath, projectPath)
        {
            ViewName = viewName;
        }

        public InstantiateTemplate(string templatePath, string projectPath, string viewName, string pageName)
            : this(templatePath, projectPath, viewName)
        {
            PageName = pageName;
        }

        private TemplateInstantiator GetInstantiator()
        {
            XTemplate xTemplate = XTemplate.Load(TemplatePath);
            Project project = ProjectExtensions.Open(ProjectPath);
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    if (ViewName != null)
                    {
                        throw new InvalidOperationException(
                            "View name cannot be specified for a project-level template.");
                    }
                    return new ProjectTemplateInstantiator(xTemplate, project);
                case TemplateLevel.View:
                    if (PageName != null)
                    {
                        throw new InvalidOperationException("Page name cannot be specified for a view-level template.");
                    }
                    if (ViewName != null)
                    {
                        XView xView = xTemplate.XProject.XViews.Single();
                        xView.Name = ViewName;
                    }
                    return new ViewTemplateInstantiator(xTemplate, project);
                case TemplateLevel.Page:
                    if (ViewName == null)
                    {
                        throw new InvalidOperationException("View name must be specified for a page-level template.");
                    }
                    if (PageName != null)
                    {
                        XView xView = xTemplate.XProject.XViews.Single();
                        XPage xPage = xView.XPages.Single();
                        xPage.Name = PageName;
                    }
                    View view = project.Views[ViewName];
                    return new PageTemplateInstantiator(xTemplate, view);
                default:
                    throw new InvalidOperationException($"Template level '{xTemplate.Level}' is not supported.");
            }
        }

        public void Run()
        {
            TemplateInstantiator instantiator = GetInstantiator();
            instantiator.Progress = Log.Progress;
            instantiator.Instantiate();
        }
    }
}
