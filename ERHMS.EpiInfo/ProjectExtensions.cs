using Epi;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common.Logging;
using ERHMS.Data.Databases;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ProjectExtensions
    {
        public static Project Create(ProjectCreationInfo info)
        {
            Log.Instance.Debug($"Creating Epi Info project: {info.FilePath}");
            Directory.CreateDirectory(info.Location);
            Project project = new Project
            {
                Id = Util.GetFileGuid(info.FilePath),
                Name = info.Name,
                Description = info.Description,
                Location = info.Location,
                CollectedDataDriver = info.Database.Type.ToDriverName(),
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

        public static Project Open(string path)
        {
            Log.Instance.Debug($"Opening Epi Info project: {path}");
            return new Project(path);
        }

        public static bool IsInitialized(this Project @this)
        {
            return @this.Metadata.TableExists("metaDbInfo");
        }

        public static void Initialize(this Project @this)
        {
            Log.Instance.Debug($"Initializing Epi Info project: {@this.FilePath}");
            ((MetadataDbProvider)@this.Metadata).CreateMetadataTables();
        }

        private static void DeleteViewCore(this Project @this, View view)
        {
            foreach (Page page in view.Pages)
            {
                @this.Metadata.DeleteFields(page);
                @this.Metadata.DeletePage(page);
            }
            @this.Metadata.DeleteView(view.Name);
            @this.Views.Remove(view.Name);
        }

        public static void DeleteViewTree(this Project @this, View view)
        {
            view.DeleteDataTableTree();
            if (view.IsRelatedView && view.ParentView != null)
            {
                foreach (RelatedViewField field in view.ParentView.Fields.RelatedFields)
                {
                    if (field.ChildView?.Id == view.Id)
                    {
                        @this.Metadata.DeleteField(field);
                    }
                }
                view.ParentView.MustRefreshFieldCollection = true;
            }
            foreach (View descendantView in view.GetDescendantViews())
            {
                @this.DeleteViewCore(descendantView);
            }
            @this.DeleteViewCore(view);
        }
    }
}
