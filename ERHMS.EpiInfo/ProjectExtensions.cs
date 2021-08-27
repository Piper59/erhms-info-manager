using Epi;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.Common.Logging;
using ERHMS.Data;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using System;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.EpiInfo
{
    public static class ProjectExtensions
    {
        public static Project Create(ProjectCreationInfo creationInfo)
        {
            Log.Instance.Debug($"Creating project: {creationInfo.FilePath}");
            if (File.Exists(creationInfo.FilePath))
            {
                throw new InvalidOperationException("Project already exists.");
            }
            Directory.CreateDirectory(creationInfo.Location);
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Name = creationInfo.Name,
                Description = creationInfo.Description,
                Location = creationInfo.Location,
                CollectedDataDriver = Configuration.GetDatabaseDriver(creationInfo.Database.Provider),
                CollectedDataConnectionString = creationInfo.Database.ConnectionString,
                CollectedDataDbInfo = new DbDriverInfo
                {
                    DBCnnStringBuilder = creationInfo.Database.GetConnectionStringBuilder(),
                    DBName = creationInfo.Database.Name
                },
                MetadataSource = MetadataSource.SameDb
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, project.CollectedDataDriver, false);
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        public static Project Open(string path)
        {
            Log.Instance.Debug($"Opening project: {path}");
            return new Project(path);
        }

        public static bool IsInitialized(this Project @this)
        {
            return @this.Metadata.TableExists(TableNames.DbInfo);
        }

        public static void Initialize(this Project @this)
        {
            Log.Instance.Debug($"Initializing project: {@this.FilePath}");
            if (@this.IsInitialized())
            {
                throw new InvalidOperationException("Project is already initialized.");
            }
            ((MetadataDbProvider)@this.Metadata).CreateMetadataTables();
        }

        public static IDatabase GetDatabase(this Project @this)
        {
            DatabaseProvider provider = Configuration.GetDatabaseProvider(@this.CollectedDataDriver);
            return provider.ToDatabase(@this.CollectedDataConnectionString);
        }

        public static IEnumerable<string> GetTableNames(this Project @this, TableTypes tableTypes)
        {
            if (tableTypes.HasFlag(TableTypes.Existing))
            {
                foreach (string tableName in @this.CollectedData.GetDbDriver().GetTableNames())
                {
                    yield return tableName;
                }
            }
            if (tableTypes.HasAnyFlag(TableTypes.View, TableTypes.Page, TableTypes.GridField))
            {
                foreach (View view in @this.Views)
                {
                    if (tableTypes.HasFlag(TableTypes.View))
                    {
                        yield return view.TableName;
                    }
                    if (tableTypes.HasFlag(TableTypes.Page))
                    {
                        foreach (Page page in view.Pages)
                        {
                            yield return page.TableName;
                        }
                    }
                    if (tableTypes.HasFlag(TableTypes.GridField))
                    {
                        foreach (GridField field in view.Fields.GridFields)
                        {
                            yield return field.TableName;
                        }
                    }
                }
            }
        }

        public static void DeletePage(this Project @this, Page page)
        {
            Log.Instance.Debug($"Deleting page: {page.DisplayName}");
            View view = page.GetView();
            @this.CollectedData.DeletePage(page);
            @this.Metadata.DeepDeletePage(page);
            view.Pages.Remove(page);
            @this.Metadata.SynchronizePageNumbersOnDelete(view, page.Position);
            view.MustRefreshFieldCollection = true;
        }

        private static void DeleteViewCore(this Project @this, View view)
        {
            Log.Instance.Debug($"Deleting view: {view.DisplayName}");
            @this.CollectedData.DeleteView(view);
            @this.Metadata.DeepDeleteView(view);
            @this.Views.Remove(view);
        }

        public static void DeleteView(this Project @this, View view)
        {
            if (view.IsRelatedView)
            {
                view.Unrelate();
            }
            @this.DeleteViewCore(view);
        }

        public static void DeleteViewTree(this Project @this, View view)
        {
            foreach (View descendantView in view.GetDescendantViews())
            {
                @this.DeleteViewCore(descendantView);
            }
            @this.DeleteViewCore(view);
        }
    }
}
