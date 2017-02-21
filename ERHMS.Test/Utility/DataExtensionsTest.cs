using ERHMS.Utility;
using NUnit.Framework;
using System.Data.SqlClient;

namespace ERHMS.Test.Utility
{
    public class DataExtensionsTest
    {
        [Test]
        public void GetCensoredConnectionStringTest()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                UserID = "jdoe",
                Password = "1234"
            };
            string connectionString = builder.GetCensoredConnectionString();
            StringAssert.Contains("jdoe", connectionString);
            StringAssert.DoesNotContain("1234", connectionString);
        }
    }
}
