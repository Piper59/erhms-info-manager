using Epi;
using ERHMS.Data;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console.Utilities
{
    public class DeleteDatabase : Utility
    {
        public string ProjectPath { get; }
        public DatabaseProvider DatabaseProvider { get; }
        public string ConnectionString { get; }

        public DeleteDatabase(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public DeleteDatabase(DatabaseProvider databaseProvider, string connectionString)
        {
            DatabaseProvider = databaseProvider;
            ConnectionString = connectionString;
        }

        private IDatabase GetDatabase()
        {
            if (ProjectPath == null)
            {
                return DatabaseProvider.ToDatabase(ConnectionString);
            }
            else
            {
                Project project = ProjectExtensions.Open(ProjectPath);
                return project.GetDatabase();
            }
        }

        public override void Run()
        {
            IDatabase database = GetDatabase();
            if (!database.Exists())
            {
                throw new InvalidOperationException("Database does not exist.");
            }
            database.Delete();
        }
    }
}
