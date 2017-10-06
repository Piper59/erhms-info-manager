using ERHMS.Dapper;
using ERHMS.Utility;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

namespace ERHMS.Test
{
    public class AccessDatabaseCreator : IDatabaseCreator
    {
        public string Name { get; private set; }
        public TempDirectory Directory { get; private set; }
        public OleDbConnectionStringBuilder Builder { get; private set; }

        public AccessDatabaseCreator(string name)
        {
            Name = name;
            Directory = new TempDirectory(Name);
            Builder = new OleDbConnectionStringBuilder
            {
                Provider = OleDbExtensions.Providers.Jet4,
                DataSource = Directory.CombinePaths(Name + ".mdb")
            };
        }

        public void SetUp()
        {
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Empty.mdb", Builder.DataSource);
        }

        public void TearDown()
        {
            Directory.Dispose();
        }

        public IDbConnection GetConnection()
        {
            return new OleDbConnection(Builder.ConnectionString);
        }

        public IDatabase GetDatabase()
        {
            return new AccessDatabase(Builder);
        }
    }
}
