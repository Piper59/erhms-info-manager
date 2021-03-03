using ERHMS.Data;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console.Utilities
{
    public class DeleteDatabase : IUtility
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
                return ProjectExtensions.Open(ProjectPath).GetDatabase();
            }
        }

        public void Run()
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
