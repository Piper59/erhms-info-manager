using System.Collections.Generic;
using System.IO;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        private static readonly IDictionary<string, string> Providers = new Dictionary<string, string>
        {
            { ".mdb", "Microsoft.Jet.OLEDB.4.0" },
            { ".accdb", "Microsoft.ACE.OLEDB.12.0" }
        };

        public static AccessDriver Create(string dataSource, string password = null)
        {
            IDictionary<string, object> connectionProperties = new Dictionary<string, object>
            {
                { "Provider", Providers[Path.GetExtension(dataSource).ToLower()] },
                { "Data Source", dataSource }
            };
            if (password != null)
            {
                connectionProperties["Jet OLEDB:Database Password"] = password;
            }
            return new AccessDriver(connectionProperties);
        }

        private AccessDriver(IDictionary<string, object> connectionProperties)
            : base(DataProvider.Access, connectionProperties)
        { }

        public override string GetParameterName(int index)
        {
            return "?";
        }
    }
}
