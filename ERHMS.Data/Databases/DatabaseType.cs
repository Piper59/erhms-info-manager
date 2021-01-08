using Epi;
using ERHMS.Common.Collections;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Data.Databases
{
    public enum DatabaseType
    {
        Access,
        SqlServer
    }

    public static class DatabaseTypeExtensions
    {
        private static readonly IReadOnlyTwoWayMap<DatabaseType, string> providerNames = new TwoWayMap<DatabaseType, string>
        {
            { DatabaseType.Access, typeof(OleDbConnection).Namespace },
            { DatabaseType.SqlServer, typeof(SqlConnection).Namespace }
        };
        private static readonly IReadOnlyTwoWayMap<DatabaseType, string> epiInfoDriverNames = new TwoWayMap<DatabaseType, string>
        {
            { DatabaseType.Access, Configuration.AccessDriver },
            { DatabaseType.SqlServer, Configuration.SqlDriver }
        };

        public static string ToProviderName(this DatabaseType @this)
        {
            return providerNames.Forward[@this];
        }

        public static DatabaseType FromProviderName(string providerName)
        {
            return providerNames.Reverse[providerName];
        }

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
