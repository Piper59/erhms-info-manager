using Dapper;
using ERHMS.Dapper;
using NUnit.Framework;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace ERHMS.Test.Dapper
{
    public abstract class TransactionTestBase
    {
        protected IDbConnection connection;

        protected void PostSetUp()
        {
            connection.Open();
            connection.Execute("CREATE TABLE Test (Id NVARCHAR(255) NOT NULL PRIMARY KEY)");
        }

        protected void PreTearDown()
        {
            connection.Dispose();
        }

        private void Insert(string id, Transaction transaction = null)
        {
            string sql = "INSERT INTO Test (Id) VALUES (@Id)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            connection.Execute(sql, parameters, transaction?.Base);
        }

        private bool Exists(string id, Transaction transaction = null)
        {
            string sql = "SELECT COUNT(*) FROM Test WHERE Id = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return connection.ExecuteScalar<int>(sql, parameters, transaction?.Base) > 0;
        }

        private void Test(string id, bool commit, bool rollback)
        {
            bool raised = false;
            using (Transaction transaction = new Transaction(connection.BeginTransaction()))
            {
                transaction.Closed += (sender, e) =>
                {
                    raised = true;
                };
                Insert(id, transaction);
                Assert.IsTrue(Exists(id, transaction));
                if (commit)
                {
                    transaction.Commit();
                }
                if (rollback)
                {
                    transaction.Rollback();
                }
            }
            Assert.IsTrue(raised);
            Assert.AreEqual(commit, Exists(id));
        }

        [Test]
        public void CommitTest()
        {
            Test(nameof(CommitTest), true, false);
        }

        [Test]
        public void ExplicitRollbackTest()
        {
            Test(nameof(ExplicitRollbackTest), false, true);
        }

        [Test]
        public void ImplicitRollbackTest()
        {
            Test(nameof(ImplicitRollbackTest), false, false);
        }
    }

    public class OleDbTransactionTest : TransactionTestBase
    {
        private TempDirectory directory;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OleDbConnectionStringBuilder builder;
            DatabaseTestBase.Access.SetUp(nameof(OleDbTransactionTest), out directory, out builder);
            connection = new OleDbConnection(builder.ConnectionString);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PreTearDown();
            directory.Dispose();
        }
    }

    public class SqlTransactionTest : TransactionTestBase
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SqlConnectionStringBuilder builder;
            DatabaseTestBase.SqlServer.SetUp(out builder);
            connection = new SqlConnection(builder.ConnectionString);
            PostSetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PreTearDown();
            DatabaseTestBase.SqlServer.TearDown();
        }
    }
}
