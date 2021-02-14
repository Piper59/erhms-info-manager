using Epi;
using ERHMS.Data;
using System;

namespace ERHMS.EpiInfo.Data
{
    public static class DatabaseProviderExtensions
    {
        public static string ToDriverName(this DatabaseProvider @this)
        {
            switch (@this)
            {
                case DatabaseProvider.Access2003:
                    return Configuration.AccessDriver;
                case DatabaseProvider.Access2007:
                    return Configuration.Access2007Driver;
                case DatabaseProvider.SqlServer:
                    return Configuration.SqlDriver;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this));
            }
        }

        public static DatabaseProvider FromDriverName(string driverName)
        {
            switch (driverName)
            {
                case Configuration.AccessDriver:
                    return DatabaseProvider.Access2003;
                case Configuration.Access2007Driver:
                    return DatabaseProvider.Access2007;
                case Configuration.SqlDriver:
                    return DatabaseProvider.SqlServer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(driverName));
            }
        }
    }
}
