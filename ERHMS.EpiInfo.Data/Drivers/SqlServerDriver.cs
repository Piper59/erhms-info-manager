using System.Collections.Generic;

namespace ERHMS.EpiInfo.Data
{
    public class SqlServerDriver : DataDriverBase
    {
        public static SqlServerDriver Create(string server, string database, string userId = null, string password = null)
        {
            IDictionary<string, object> connectionProperties = new Dictionary<string, object>
            {
                { "Server", server },
                { "Database", database }
            };
            if (userId == null && password == null)
            {
                connectionProperties["Integrated Security"] = "true";
            }
            else
            {
                if (userId != null)
                {
                    connectionProperties["User ID"] = userId;
                }
                if (password != null)
                {
                    connectionProperties["Password"] = password;
                }
            }
            return new SqlServerDriver(connectionProperties);
        }

        private SqlServerDriver(IDictionary<string, object> connectionProperties)
            : base(DataProvider.SqlServer, connectionProperties)
        { }

        public override string GetParameterName(int index)
        {
            return string.Format("@P{0}", index);
        }
    }
}
