using Epi;
using ERHMS.Common;
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

        public InstantiateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        protected override void RunCore()
        {
            XTemplate xTemplate = new XTemplate(XDocument.Load(TemplatePath).Root);
            Project project = new Project(ProjectPath);
            TemplateInstantiator instantiator;
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    instantiator = new ProjectTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.View:
                    instantiator = new ViewTemplateInstantiator(xTemplate, project);
                    break;
                default:
                    throw new ArgumentException($"Template level '{xTemplate.Level}' is not supported.");
            }
            instantiator.Progress = new ProgressLogger();
            instantiator.Instantiate();
            Log.Default.Debug("Template has been instantiated");
        }
    }
}
