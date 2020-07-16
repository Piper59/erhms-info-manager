using Epi;
using System;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public static class DatabaseFactory
    {
        public static IDatabase GetDatabase(DbConnectionStringBuilder builder)
        {
            if (builder is OleDbConnectionStringBuilder oleDbBuilder)
            {
                return new AccessDatabase(oleDbBuilder);
            }
            if (builder is SqlConnectionStringBuilder sqlBuilder)
            {
                return new SqlServerDatabase(sqlBuilder);
            }
            throw new NotSupportedException();
        }

        public static IDatabase GetDatabase(string driver, string connectionString)
        {
            DatabaseType type = DatabaseTypeExtensions.FromDriver(driver);
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

        public static IDatabase GetDatabase(Project project)
        {
            return GetDatabase(project.CollectedDataDriver, project.CollectedDataConnectionString);
        }
    }
}
