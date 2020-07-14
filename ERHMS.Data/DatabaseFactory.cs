using Epi;
using ERHMS.Utility;
using System;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public static class DatabaseFactory
    {
        public static IDatabase GetDatabase(DbConnectionStringBuilder connectionStringBuilder)
        {
            if (connectionStringBuilder is OleDbConnectionStringBuilder oleDbConnectionStringBuilder)
            {
                return new AccessDatabase(oleDbConnectionStringBuilder);
            }
            if (connectionStringBuilder is SqlConnectionStringBuilder sqlConnectionStringBuilder)
            {
                return new SqlServerDatabase(sqlConnectionStringBuilder);
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
                    throw new InvalidEnumValueException(type);
            }
        }

        public static IDatabase GetDatabase(Project project)
        {
            return GetDatabase(project.CollectedDataDriver, project.CollectedDataConnectionString);
        }
    }
}
