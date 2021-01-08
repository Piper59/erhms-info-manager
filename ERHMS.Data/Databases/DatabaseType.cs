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
        private static readonly ITwoWayMap<DatabaseType, string> epiInfoDriverNames = new TwoWayMap<DatabaseType, string>
        {
            { DatabaseType.Access, Configuration.AccessDriver },
            { DatabaseType.SqlServer, Configuration.SqlDriver }
        };

        public static string ToEpiInfoDriverName(this DatabaseType @this)
        {
            return epiInfoDriverNames.Forward[@this];
        }

        public static DatabaseType FromEpiInfoDriverName(string epiInfoDriverName)
        {
            return epiInfoDriverNames.Reverse[epiInfoDriverName];
        }
    }
}
