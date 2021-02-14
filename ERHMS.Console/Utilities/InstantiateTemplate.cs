using Epi;
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

        public void Run()
        {
            XTemplate xTemplate = XTemplate.Load(TemplatePath);
            Project project = ProjectExtensions.Open(ProjectPath);
            TemplateInstantiator instantiator;
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    if (ViewName != null)
                    {
                        throw new InvalidOperationException(
                            "View name cannot be specified for a project-level template.");
                    }
                    instantiator = new ProjectTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.View:
                    instantiator = new ViewTemplateInstantiator(xTemplate, project);
                    if (ViewName != null)
                    {
                        XView xView = xTemplate.XProject.XViews.Single();
                        xView.Name = ViewName;
                    }
                    break;
                case TemplateLevel.Page:
                    if (ViewName == null)
                    {
                        throw new InvalidOperationException("View name must be specified for a page-level template.");
                    }
                    View view = project.Views[ViewName];
                    instantiator = new PageTemplateInstantiator(xTemplate, view);
                    break;
                default:
                    throw new InvalidOperationException($"Template level '{xTemplate.Level}' is not supported.");
            }
            instantiator.Progress = new Progress<string>(Log.Default.Debug);
            instantiator.Instantiate();
        }
    }
}
