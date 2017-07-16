using Dapper;
using ERHMS.Dapper;
using ERHMS.Utility;
using NUnit.Framework;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

namespace ERHMS.Test.Dapper
{
    public class TransactionTest
    {
        private TempDirectory directory;
        private IDbConnection connection;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(TransactionTest));
            string path = directory.CombinePaths(nameof(TransactionTest) + ".mdb");
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", path);
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.AccessProvider,
                DataSource = path
            };
            connection = new OleDbConnection(builder.ConnectionString);
            connection.Open();
            connection.Execute("CREATE TABLE Test (Id NVARCHAR(255) NOT NULL PRIMARY KEY)");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            connection.Dispose();
            directory.Dispose();
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
            return connection.ExecuteScalar<int>(sql, parameters, transaction?.Base) != 0;
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
}
