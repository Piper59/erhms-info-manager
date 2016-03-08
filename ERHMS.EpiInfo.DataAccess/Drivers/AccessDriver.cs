using System.Collections.Generic;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        private const string Provider = "Microsoft.Jet.OLEDB.4.0";

        public static AccessDriver Create(string dataSource, string password = null)
        {
            IDictionary<string, object> connectionProperties = new Dictionary<string, object>
            {
                { "Provider", Provider },
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
