using ERHMS.Data.Properties;
using System.Data.OleDb;
using System.IO;

namespace ERHMS.Data.Databases
{
    public class AccessDatabase : Database<OleDbConnectionStringBuilder, OleDbConnection>
    {
        public override DatabaseType Type => DatabaseType.Access;
        public string FilePath => ConnectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

        public AccessDatabase(string connectionString)
            : base(connectionString) { }

        protected override string GetId(OleDbConnectionStringBuilder connectionStringBuilder)
        {
            return connectionStringBuilder.DataSource;
        }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        protected override void CreateCore()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream stream = File.Open(FilePath, FileMode.CreateNew, FileAccess.Write))
            {
                stream.Write(ResX.AccessDatabase, 0, ResX.AccessDatabase.Length);
            }
        }
    }
}
