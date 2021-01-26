using Epi;
using ERHMS.Common.Collections;

namespace ERHMS.Data.Databases
{
    public enum DatabaseType
    {
        Access,
        SqlServer
    }

    public static class DatabaseTypeExtensions
    {
        private static readonly ITwoWayMap<DatabaseType, string> driverNames = new TwoWayMap<DatabaseType, string>
        {
            { DatabaseType.Access, Configuration.AccessDriver },
            { DatabaseType.SqlServer, Configuration.SqlDriver }
        };

        public static string ToDriverName(this DatabaseType @this)
        {
            return driverNames.Forward[@this];
        }

        public static DatabaseType FromDriverName(string driverName)
        {
            return driverNames.Reverse[driverName];
        }
    }
}
