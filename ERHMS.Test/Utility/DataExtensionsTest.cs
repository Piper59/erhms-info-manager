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
            string connectionString = new SqlConnectionStringBuilder
            {
                UserID = "jdoe",
                Password = "1234"
            }.GetCensoredConnectionString();
            StringAssert.Contains("jdoe", connectionString);
            StringAssert.DoesNotContain("1234", connectionString);
        }
    }
}
