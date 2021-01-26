using Epi;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Xml.Linq;

namespace ERHMS.Console.Utilities
{
    public class InstantiateTemplate : Utility
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

        protected override void RunCore()
        {
            XTemplate xTemplate = new XTemplate(XDocument.Load(TemplatePath).Root);
            Project project = ProjectExtensions.Open(ProjectPath);
            TemplateInstantiator instantiator;
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    if (ViewName != null)
                    {
                        throw new ArgumentException("View name cannot be specified for a project-level template.");
                    }
                    instantiator = new ProjectTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.View:
                    instantiator = new ViewTemplateInstantiator(xTemplate, project);
                    if (ViewName != null)
                    {
                        xTemplate.XProject.XView.Name = ViewName;
                    }
                    break;
                case TemplateLevel.Page:
                    if (ViewName != null)
                    {
                        throw new ArgumentException("View name must be specified for a page-level template.");
                    }
                    instantiator = new PageTemplateInstantiator(xTemplate, project.Views[ViewName]);
                    break;
                default:
                    throw new ArgumentException($"Template level '{xTemplate.Level}' is not supported.");
            }
            instantiator.Progress = new LoggingProgress();
            instantiator.Instantiate();
        }
    }
}
