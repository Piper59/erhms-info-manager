using Epi;
using ERHMS.Data;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templates;
using ERHMS.EpiInfo.Templates.Xml;
using System;
using System.Data.OleDb;
using System.IO;
using System.Xml.Linq;

namespace ERHMS.Console.Utilities
{
    public class InstantiateTemplate : Utility
    {
        public string TemplatePath { get; }
        public string ProjectLocation { get; }
        public string ProjectName { get; }

        public InstantiateTemplate(string templatePath, string projectLocation, string projectName)
        {
            TemplatePath = templatePath;
            ProjectLocation = projectLocation;
            ProjectName = projectName;
        }

        protected override void RunCore()
        {
            ProjectCreationInfo info = new ProjectCreationInfo
            {
                Location = ProjectLocation,
                Name = ProjectName
            };
            if (File.Exists(info.FilePath))
            {
                throw new ArgumentException("Project already exists.");
            }
            string databasePath = Path.ChangeExtension(info.FilePath, AccessDatabase.FileExtension);
            IDatabase database = new AccessDatabase(new OleDbConnectionStringBuilder
            {
                DataSource = databasePath
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
            info.Database = database;
            Project project = ProjectExtensions.Create(info);
            project.Initialize();
            ProjectTemplateInstantiator instantiator = new ProjectTemplateInstantiator(xTemplate, project)
            {
                Progress = new ProgressLogger()
            };
            instantiator.Instantiate();
            Log.Default.Debug("Template has been instantiated");
        }
    }
}
