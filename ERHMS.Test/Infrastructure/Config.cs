using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ERHMS.Test
{
    public static class Config
    {
        public static Uri Endpoint
        {
            get { return new Uri(ConfigurationManager.AppSettings["Endpoint"]); }
        }

        public static Guid OrganizationKey
        {
            get { return new Guid(ConfigurationManager.AppSettings["OrganizationKey"]); }
        }

        private static string TestConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString; }
        }

        public static SqlConnection GetMasterConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(TestConnectionString)
            {
                InitialCatalog = "master"
            };
            return new SqlConnection(builder.ConnectionString);
        }

        public static SqlConnectionStringBuilder GetTestConnectionStringBuilder()
        {
            return new SqlConnectionStringBuilder(TestConnectionString)
            {
                Pooling = false
            };
        }

        public static SqlConnection GetWebConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["EIWS"].ConnectionString);
        }
    }
}
