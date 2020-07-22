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
        public string ProjectLocation { get; }
        public string ProjectName { get; }

        public InstantiateTemplate(string templatePath, string projectLocation, string projectName)
        {
            TemplatePath = templatePath;
            ProjectLocation = projectLocation;
            ProjectName = projectName;
        }

        protected override async Task<string> RunCoreAsync()
        {
            await Task.Run(() =>
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
                Progress?.Report($"Creating database: {databasePath}");
                database.Create();
                info.Database = database;
                Progress?.Report($"Creating project: {info.FilePath}");
                Project project = ProjectExtensions.Create(info);
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
