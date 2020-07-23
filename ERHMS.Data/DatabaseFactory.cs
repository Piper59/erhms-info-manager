using Epi;
using System;

namespace ERHMS.Data
{
    public static class DatabaseFactory
    {
        public static IDatabase GetDatabase(DatabaseType type, string connectionString)
        {
            switch (type)
            {
                case DatabaseType.Access:
                    return new AccessDatabase(connectionString);
                case DatabaseType.SqlServer:
                    return new SqlServerDatabase(connectionString);
                default:
                    throw new NotSupportedException();
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
