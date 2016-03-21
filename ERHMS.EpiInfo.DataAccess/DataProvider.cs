using Epi;
using System;

namespace ERHMS.EpiInfo.DataAccess
{
    public enum DataProvider
    {
        Access,
        SqlServer
    }

    public static class DataProviderExtensions
    {
        public static string GetInvariantName(this DataProvider @this)
        {
            switch (@this)
            {
                case DataProvider.Access:
                    return "System.Data.OleDb";
                case DataProvider.SqlServer:
                    return "System.Data.SqlClient";
                default:
                    throw new NotSupportedException();
            }
        }

        public static string GetEpiInfoName(this DataProvider @this)
        {
            switch (@this)
            {
                case DataProvider.Access:
                    return Configuration.AccessDriver;
                case DataProvider.SqlServer:
                    return Configuration.SqlDriver;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
