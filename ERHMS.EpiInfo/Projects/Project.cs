using Epi;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;
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
                Id = Util.GetFileGuid(info.FilePath),
                Name = info.Name,
                Description = info.Description,
                Location = info.Location,
                CollectedDataDriver = info.Database.Type.ToDriver(),
                CollectedDataConnectionString = info.Database.ConnectionString,
                CollectedDataDbInfo = new DbDriverInfo
                {
                    DBCnnStringBuilder = info.Database.GetConnectionStringBuilder(),
                    DBName = info.Database.Name
                }
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
        public virtual ProjectType Type => ProjectType.Unknown;
        public IEnumerable<CoreView> CoreViews => CoreView.All.Where(coreView => coreView.ProjectType == Type);

        public Project() { }

        public Project(string path)
            : base(path)
        {
            Log.Default.Debug($"Opening project: {path}");
        }

        public bool IsInitialized()
        {
            return Metadata.TableExists("metaDbInfo");
        }

        public void Initialize()
        {
            Log.Default.Debug("Initializing");
            Metadata.CreateMetadataTables();
        }

        public View InstantiateView(XTemplate xTemplate, IProgress<string> progress = null)
        {
            Log.Default.Debug($"Instantiating view: {xTemplate.Name}");
            ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, this)
            {
                Progress = new ProgressLogger(progress)
            };
            instantiator.Instantiate();
            return instantiator.View;
        }

        public View InstantiateView(CoreView coreView)
        {
            Log.Default.Debug($"Instantiating core view: {coreView.Name}");
            string resourceName = $"ERHMS.Resources.Templates.Forms.{coreView.ProjectType}.{coreView.Name}.xml";
            XDocument document;
            using (Stream stream = ResourceProvider.GetResource(resourceName))
            {
                document = XDocument.Load(stream);
            }
            XTemplate xTemplate = new XTemplate(document.Root);
            return InstantiateView(xTemplate);
        }

        private void DeleteViewInternal(View view)
        {
            foreach (Page page in view.Pages)
            {
                Metadata.DeleteFields(page);
                Metadata.DeletePage(page);
            }
            Metadata.DeleteView(view.Name);
            Views.Remove(view.Name);
        }

        public void DeleteView(View view)
        {
            view.DeleteAllDataTables();
            if (view.IsRelatedView && view.ParentView != null)
            {
                foreach (RelatedViewField field in view.ParentView.Fields.RelatedFields)
                {
                    if (field.ChildView?.Id == view.Id)
                    {
                        metadata.DeleteField(field);
                    }
                }
                view.ParentView.MustRefreshFieldCollection = true;
            }
            foreach (View descendantView in view.GetDescendantViews())
            {
                DeleteViewInternal(descendantView);
            }
            DeleteViewInternal(view);
        }
    }
}
