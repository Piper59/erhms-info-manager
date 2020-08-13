using Epi;
using ERHMS.Common;
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
        public string FormName { get; }

        public InstantiateTemplate(string templatePath, string projectPath, string formName)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
            FormName = formName;
        }

        protected override void RunCore()
        {
            XTemplate xTemplate = new XTemplate(XDocument.Load(TemplatePath).Root);
            Project project = new Project(ProjectPath);
            TemplateInstantiator instantiator;
            switch (xTemplate.Level)
            {
                case TemplateLevel.Project:
                    if (FormName != "")
                    {
                        throw new ArgumentException("Form name must be empty (\"\") for a project-level template.");
                    }
                    instantiator = new ProjectTemplateInstantiator(xTemplate, project);
                    break;
                case TemplateLevel.View:
                    instantiator = new ViewTemplateInstantiator(xTemplate, project);
                    xTemplate.XProject.XView.SetName(FormName);
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
