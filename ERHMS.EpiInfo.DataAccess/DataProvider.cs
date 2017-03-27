using Epi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public enum DataProvider
    {
        [Description("Microsoft Access")]
        Access,

        [Description("Microsoft SQL Server")]
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

        public static DataProvider FromInvariantName(string value)
        {
            return InvariantNames.Single(pair => pair.Value == value).Key;
        }

        public static string ToEpiInfoName(this DataProvider @this)
        {
            return EpiInfoNames.Single(pair => pair.Key == @this).Value;
        }

        public static DataProvider FromEpiInfoName(string value)
        {
            return EpiInfoNames.Single(pair => pair.Value == value).Key;
        }
    }
}
