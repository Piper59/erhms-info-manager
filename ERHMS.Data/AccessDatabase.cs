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
        public string FilePath => connectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

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
            return File.Exists(FilePath);
        }

        public override void CreateCore()
        {
            string resourceName = "ERHMS.Data.Resources.Empty.mdb";
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (Stream target = File.Create(FilePath))
            {
                source.CopyTo(target);
            }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
