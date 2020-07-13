using Epi;
using ERHMS.Utility;

namespace ERHMS.Data
{
    public static class DatabaseFactory
    {
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
