using Epi;
using ERHMS.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templates;
using ERHMS.EpiInfo.Templates.Xml;
using System;
using System.Data.OleDb;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ERHMS.Desktop.Utilities
{
    public class InstantiateTemplate : Utility
    {
        public override bool LongRunning => true;
        public string TemplatePath { get; }
        public string ProjectPath { get; }

        public InstantiateTemplate(string templatePath, string projectPath)
        {
            TemplatePath = templatePath;
            ProjectPath = projectPath;
        }

        protected override async Task<string> RunCoreAsync()
        {
            await Task.Run(() =>
            {
                if (File.Exists(ProjectPath))
                {
                    throw new ArgumentException("Project already exists.");
                }
                IDatabase database = new AccessDatabase(new OleDbConnectionStringBuilder
                {
                    DataSource = Path.ChangeExtension(ProjectPath, AccessDatabase.FileExtension)
                });
                if (database.Exists())
                {
                    throw new ArgumentException("Database already exists.");
                }
                XTemplate xTemplate = new XTemplate(XDocument.Load(TemplatePath).Root);
                if (xTemplate.Level != TemplateLevel.Project)
                {
                    throw new ArgumentException("Template is not project-level.");
                }
                database.Create();
                Project project = ProjectExtensions.Create(new ProjectCreationInfo
                {
                    Name = Path.GetFileNameWithoutExtension(ProjectPath),
                    Location = Path.GetDirectoryName(ProjectPath),
                    Database = database
                });
                project.Initialize();
                ProjectTemplateInstantiator instantiator = new ProjectTemplateInstantiator(xTemplate, project)
                {
                    Progress = Progress
                };
                instantiator.Instantiate();
            });
            return "Template has been instantiated.";
        }
    }
}
