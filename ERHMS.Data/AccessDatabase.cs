using ERHMS.Utility;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace ERHMS.Data
{
    public class AccessDatabase : DatabaseBase
    {
        private OleDbConnectionStringBuilder connectionStringBuilder;
        private OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder();

        public override DatabaseType Type => DatabaseType.Access;
        public override DbConnectionStringBuilder ConnectionStringBuilder => connectionStringBuilder;
        protected override DbCommandBuilder CommandBuilder => commandBuilder;
        public string FilePath => connectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

        public AccessDatabase(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
        }

        public AccessDatabase(string connectionString)
            : this(new OleDbConnectionStringBuilder(connectionString)) { }

        protected override DbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        public override void CreateCore()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Data.Resources.Empty.mdb", FilePath);
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
