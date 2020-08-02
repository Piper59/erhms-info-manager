using Epi;
using System;

namespace ERHMS.Data.Databases
{
    public static class DatabaseFactory
    {
        public static IDatabase GetDatabase(DatabaseType databaseType, string connectionString)
        {
            switch (databaseType)
            {
                case DatabaseType.Access:
                    return new AccessDatabase(connectionString);
                case DatabaseType.SqlServer:
                    return new SqlServerDatabase(connectionString);
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType));
            }
        }

        public static IDatabase GetDatabase(string driver, string connectionString)
        {
            return GetDatabase(DatabaseTypeExtensions.FromDriver(driver), connectionString);
        }

        public static IDatabase GetDatabase(Project project)
        {
            return GetDatabase(project.CollectedDataDriver, project.CollectedDataConnectionString);
        }
    }
}
