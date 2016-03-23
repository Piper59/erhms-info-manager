using Epi;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public enum DataProvider
    {
        Access,
        SqlServer
    }

    public static class DataProviderExtensions
    {
        private static readonly IDictionary<DataProvider, string> InvariantNames = new Dictionary<DataProvider, string>
        {
            { DataProvider.Access, "System.Data.OleDb" },
            { DataProvider.SqlServer, "System.Data.SqlClient" }
        };
        private static readonly IDictionary<DataProvider, string> EpiInfoNames = new Dictionary<DataProvider, string>
        {
            { DataProvider.Access, Configuration.AccessDriver },
            { DataProvider.SqlServer, Configuration.SqlDriver }
        };

        public static string ToInvariantName(this DataProvider @this)
        {
            return InvariantNames.Single(pair => pair.Key == @this).Value;
        }

        public static DataProvider FromInvariantName(string invariantName)
        {
            return InvariantNames.Single(pair => pair.Value == invariantName).Key;
        }

        public static string ToEpiInfoName(this DataProvider @this)
        {
            return EpiInfoNames.Single(pair => pair.Key == @this).Value;
        }

        public static DataProvider FromEpiInfoName(string epiInfoName)
        {
            return EpiInfoNames.Single(pair => pair.Value == epiInfoName).Key;
        }
    }
}
