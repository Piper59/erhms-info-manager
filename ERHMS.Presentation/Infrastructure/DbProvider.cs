using Epi;
using ERHMS.Utility;
using System.ComponentModel;

namespace ERHMS.Presentation
{
    public enum DbProvider
    {
        [Description("Microsoft Access")]
        Access,

        [Description("Microsoft SQL Server")]
        SqlServer
    }

    public static class DbProviderExtensions
    {
        public static readonly TwoWayDictionary<DbProvider, string> EpiInfoValues = new TwoWayDictionary<DbProvider, string>
        {
            { DbProvider.Access, Configuration.AccessDriver },
            { DbProvider.SqlServer, Configuration.SqlDriver }
        };
    }
}
