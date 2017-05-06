using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public abstract class DataDriverTestBase
    {
        protected IDataDriver driver;

        [Test]
        [Order(1)]
        public void CreateDatabaseTest()
        {
            Assert.IsFalse(driver.DatabaseExists());
            driver.CreateDatabase();
            Assert.IsTrue(driver.DatabaseExists());
            driver.ExecuteScript(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Person.sql"));
        }

        [Test]
        public void TableExistsTest()
        {
            Assert.IsTrue(driver.TableExists("Person"));
            Assert.IsFalse(driver.TableExists("People"));
        }

        [Test]
        public void ExecuteScalarTest()
        {
            string sql = "SELECT [Name] FROM [Gender] WHERE [GenderId] = 1";
            Assert.AreEqual("Male", driver.ExecuteScalar<string>(sql));
            DataQueryBuilder builder = new DataQueryBuilder(driver);
            builder.Sql.Append("SELECT [BirthDate] FROM [Person] WHERE [PersonId] = {@}");
            builder.Values.Add("999181b4-8445-e585-5178-74a9e11e75fa");
            Assert.AreEqual(new DateTime(1986, 9, 14), driver.ExecuteScalar<DateTime>(builder.GetQuery()));
        }

        [Test]
        public void ExecuteQueryTest()
        {
            string sql = "SELECT * FROM [Person] WHERE [GenderId] = 1";
            Assert.AreEqual(51, driver.ExecuteQuery(sql).Rows.Count);
            DataQueryBuilder builder = new DataQueryBuilder(driver);
            builder.Sql.Append("SELECT * FROM [Person] WHERE [Height] > {@}");
            builder.Values.Add(6.0);
            Assert.AreEqual(8, driver.ExecuteQuery(builder.GetQuery()).Rows.Count);
        }

        [Test]
        public void ExecuteNonQueryTest()
        {
            using (DataTransaction transaction = driver.BeginTransaction())
            {
                {
                    DataQueryBuilder builder = new DataQueryBuilder(driver, transaction);
                    builder.Sql.Append("UPDATE [Person] SET [Weight] = NULL");
                    driver.ExecuteNonQuery(builder.GetQuery());
                }
                {
                    DataQueryBuilder builder = new DataQueryBuilder(driver, transaction);
                    builder.Sql.Append("SELECT COUNT(*) FROM [Person] WHERE [Weight] IS NULL");
                    Assert.AreEqual(100, driver.ExecuteScalar<int>(builder.GetQuery()));
                }
                transaction.Rollback();
            }
            {
                string sql = "SELECT COUNT(*) FROM [Person] WHERE [Weight] IS NULL";
                Assert.AreEqual(0, driver.ExecuteScalar<int>(sql));
            }
            using (DataTransaction transaction = driver.BeginTransaction())
            {
                DataQueryBuilder builder = new DataQueryBuilder(driver, transaction);
                builder.Sql.Append("INSERT INTO [Person] ([PersonId], [GenderId], [LastName]) VALUES ({@}, {@}, {@})");
                builder.Values.Add("02448489-3724-4264-bdca-0e80594976ff");
                builder.Values.Add(2);
                builder.Values.Add("Doe");
                driver.ExecuteNonQuery(builder.GetQuery());
                transaction.Commit();
            }
            {
                string sql = "SELECT COUNT(*) FROM [Person]";
                Assert.AreEqual(101, driver.ExecuteScalar<int>(sql));
            }
        }
    }

    public class AccessDriverTest : DataDriverTestBase
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(AccessDriverTest));
            driver = AccessDriver.Create(directory.CombinePaths(nameof(AccessDriverTest) + ".mdb"));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            directory.Dispose();
        }
    }

    public class SqlServerDriverTest : DataDriverTestBase
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false
            };
            driver = new SqlServerDriver(builder);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
            SqlClientExtensions.ExecuteMaster(ConnectionString, "DROP DATABASE {0}", builder.InitialCatalog);
        }
    }
}
