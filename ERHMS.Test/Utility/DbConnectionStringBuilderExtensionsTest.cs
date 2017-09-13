using ERHMS.Utility;
using NUnit.Framework;
using System.Data.SqlClient;

namespace ERHMS.Test.Utility
{
    public class DbConnectionStringBuilderExtensionsTest
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
            StringAssert.Contains(builder.UserID, connectionString);
            StringAssert.DoesNotContain(builder.Password, connectionString);
        }
    }
}
