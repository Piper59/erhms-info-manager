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

        public void Run()
        {
            IDatabase database = ProjectPath == null
                ? DatabaseProvider.ToDatabase(ConnectionString)
                : ProjectExtensions.Open(ProjectPath).GetDatabase();
            if (!database.Exists())
            {
                throw new InvalidOperationException("Database does not exist.");
            }
            database.Delete();
        }
    }
}
