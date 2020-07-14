using ERHMS.Utility;
using System.Data;
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
        public string Path => connectionStringBuilder.DataSource;
        public override string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

        public AccessDatabase(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
        }

        public AccessDatabase(string connectionString)
            : this(new OleDbConnectionStringBuilder(connectionString)) { }

        protected override IDbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        public override bool Exists()
        {
            return File.Exists(Path);
        }

        public override void CreateCore()
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Data.Resources.Empty.mdb", Path);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
