using Epi;
using Epi.Data;
using Epi.Data.Services;
using ERHMS.Common;
using ERHMS.Data.Databases;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Projects
{
    public class Project : Epi.Project
    {
        protected static TProject Create<TProject>(ProjectCreationInfo info)
            where TProject : Project, new()
        {
            Log.Default.Debug($"Creating project: {info.FilePath}");
            Directory.CreateDirectory(info.Location);
            TProject project = new TProject
            {
                Id = Guid.NewGuid(),
                Name = info.Name,
                Description = info.Description,
                Location = info.Location,
                CollectedDataDriver = info.Database.Type.ToDriver(),
                CollectedDataConnectionString = info.Database.ConnectionString,
                CollectedDataDbInfo = new DbDriverInfo
                {
                    DBCnnStringBuilder = info.Database.GetConnectionStringBuilder(),
                    DBName = info.Database.Name
                },
                Database = info.Database
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, project.CollectedDataDriver, false);
            project.MetadataSource = MetadataSource.SameDb;
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        public static Project Create(ProjectCreationInfo info)
        {
            return Create<Project>(info);
        }

        public new MetadataDbProvider Metadata => (MetadataDbProvider)base.Metadata;
        public IDatabase Database { get; private set; }
        public virtual ProjectType Type => ProjectType.Unknown;
        protected virtual ICollection<string> BuiltInViewNames { get; } = new string[] { };

        public Project() { }

        public Project(string path)
            : base(path)
        {
            Log.Default.Debug($"Opening project: {path}");
            Database = DatabaseFactory.GetDatabase(this);
        }

        public bool IsInitialized()
        {
            return Database.TableExists("metaDbInfo");
        }

        public void InitializeMetadata()
        {
            Log.Default.Debug("Initializing metadata");
            Metadata.CreateMetadataTables();
        }

        public View InitializeBuiltInView(string viewName)
        {
            Log.Default.Debug($"Initializing built-in view: {viewName}");
            XTemplate xTemplate;
            string resourceName = $"ERHMS.Resources.Templates.Forms.{Type}.{viewName}.xml";
            using (Stream stream = ResourceProvider.GetResource(resourceName))
            {
                XDocument document = XDocument.Load(stream);
                xTemplate = new XTemplate(document.Root);
            }
            ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, this)
            {
                Progress = new ProgressLogger()
            };
            instantiator.Instantiate();
            return instantiator.View;
        }

        public ISet<string> GetTableNameSet()
        {
            return new HashSet<string>(Database.GetTableNames(), StringComparer.OrdinalIgnoreCase);
        }

        public ISet<string> GetViewNameSet()
        {
            return new HashSet<string>(Views.Cast<View>().Select(view => view.Name), StringComparer.OrdinalIgnoreCase);
        }
    }
}
