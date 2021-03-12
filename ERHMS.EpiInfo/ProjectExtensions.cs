using Epi;
using Epi.Data;
using Epi.Data.Services;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.Data;
using ERHMS.EpiInfo.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using DatabaseProviderExtensions = ERHMS.EpiInfo.Data.DatabaseProviderExtensions;

namespace ERHMS.EpiInfo
{
    public static class ProjectExtensions
    {
        private static DbDriverInfo GetDriverInfo(IDatabase database)
        {
            DbConnectionStringBuilder connectionStringBuilder = database.Provider.ToProviderFactory()
                .CreateConnectionStringBuilder();
            connectionStringBuilder.ConnectionString = database.ConnectionString;
            return new DbDriverInfo
            {
                DBCnnStringBuilder = connectionStringBuilder,
                DBName = database.Name
            };
        }

        public static Project Create(ProjectCreationInfo creationInfo)
        {
            Log.Default.Debug($"Creating project: {creationInfo.FilePath}");
            Directory.CreateDirectory(creationInfo.Location);
            Project project = new Project
            {
                Id = Guid.NewGuid(),
                Name = creationInfo.Name,
                Description = creationInfo.Description,
                Location = creationInfo.Location,
                CollectedDataDriver = creationInfo.Database.Provider.ToDriverName(),
                CollectedDataConnectionString = creationInfo.Database.ConnectionString,
                CollectedDataDbInfo = GetDriverInfo(creationInfo.Database),
                MetadataSource = MetadataSource.SameDb
            };
            project.CollectedData.Initialize(project.CollectedDataDbInfo, project.CollectedDataDriver, false);
            project.Metadata.AttachDbDriver(project.CollectedData.GetDbDriver());
            project.Save();
            return project;
        }

        public static bool IsInitialized(this Project @this)
        {
            return @this.Metadata.TableExists("metaDbInfo");
        }

        public static void Initialize(this Project @this)
        {
            Log.Default.Debug($"Initializing project: {@this.FilePath}");
            ((MetadataDbProvider)@this.Metadata).CreateMetadataTables();
        }

        public static Project Open(string path)
        {
            Log.Default.Debug($"Opening project: {path}");
            return new Project(path);
        }

        public static IDatabase GetDatabase(this Project @this)
        {
            return DatabaseProviderExtensions.FromDriverName(@this.CollectedDataDriver)
                .ToDatabase(@this.CollectedDataConnectionString);
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
            if (tableTypes.HasFlag(TableTypes.View)
                || tableTypes.HasFlag(TableTypes.Page)
                || tableTypes.HasFlag(TableTypes.GridField))
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

        private static void DeleteViewCore(this Project @this, View view)
        {
            Log.Default.Debug($"Deleting view: {view.DisplayName}");
            view.DeleteDataTables();
            view.DeleteMetadata();
            @this.Views.Remove(view);
        }

        public static void DeleteViewTree(this Project @this, View view)
        {
            Log.Default.Debug($"Deleting view tree: {view.DisplayName}");
            view.Unrelate();
            foreach (View descendantView in view.GetDescendantViews())
            {
                @this.DeleteViewCore(descendantView);
            }
            @this.DeleteViewCore(view);
        }
    }
}
