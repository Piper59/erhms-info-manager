using ERHMS.Data.Access;
using ERHMS.Data.SqlServer;
using System;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Data
{
    public enum DatabaseProvider
    {
        Access2003,
        Access2007,
        SqlServer
    }

    public static class DatabaseProviderExtensions
    {
        public static DbProviderFactory ToProviderFactory(this DatabaseProvider @this)
        {
            switch (@this)
            {
                case DatabaseProvider.Access2003:
                case DatabaseProvider.Access2007:
                    return OleDbFactory.Instance;
                case DatabaseProvider.SqlServer:
                    return SqlClientFactory.Instance;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static IDatabase ToDatabase(this DatabaseProvider @this, string connectionString)
        {
            switch (@this)
            {
                case DatabaseProvider.Access2003:
                    return new Access2003Database(connectionString);
                case DatabaseProvider.Access2007:
                    return new Access2007Database(connectionString);
                case DatabaseProvider.SqlServer:
                    return new SqlServerUserDatabase(connectionString);
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }
    }
}
