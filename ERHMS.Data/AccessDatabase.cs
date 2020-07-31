using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace ERHMS.Data
{
    public class AccessDatabase : Database
    {
        public const string Provider = "Microsoft.Jet.OLEDB.4.0";
        public const string FileExtension = ".mdb";
        private const string ResourceName = "ERHMS.Data.Resources.AccessDatabase.mdb";

        private readonly OleDbConnectionStringBuilder connectionStringBuilder;

        public override DatabaseType Type => DatabaseType.Access;
        protected override DbConnectionStringBuilder ConnectionStringBuilder => connectionStringBuilder;
        public string FilePath => connectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

        public AccessDatabase(string connectionString)
        {
            connectionStringBuilder = new OleDbConnectionStringBuilder(connectionString)
            {
                Provider = Provider
            };
            connectionStringBuilder.DataSource = Path.GetFullPath(connectionStringBuilder.DataSource);
        }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        protected override void CreateCore()
        {
            using (Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                using (Stream target = File.Create(FilePath))
                {
                    source.CopyTo(target);
                }
            }
        }

        protected override IDbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
