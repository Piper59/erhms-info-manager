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

        private OleDbConnectionStringBuilder builder;
        private OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder();

        public override DatabaseType Type => DatabaseType.Access;
        public override DbConnectionStringBuilder Builder => builder;
        protected override DbCommandBuilder CommandBuilder => commandBuilder;
        public string FilePath => Path.GetFullPath(builder.DataSource);
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

        public AccessDatabase(string connectionString)
        {
            builder = new OleDbConnectionStringBuilder(connectionString)
            {
                Provider = Provider
            };
        }

        protected override IDbConnection GetConnection()
        {
            return new OleDbConnection(ConnectionString);
        }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        protected override void CreateCore()
        {
            string resourceName = "ERHMS.Data.Resources.Empty.mdb";
            using (Stream source = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                using (Stream target = File.Create(FilePath))
                {
                    source.CopyTo(target);
                }
            }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
