using System.Data.OleDb;
using System.IO;

namespace ERHMS.Data.Access
{
    public abstract class AccessDatabase : Database
    {
        public static string GetConnectionString(string provider = "", string dataSource = "")
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            if (!string.IsNullOrEmpty(provider))
            {
                builder.Provider = provider;
            }
            if (!string.IsNullOrEmpty(dataSource))
            {
                builder.DataSource = dataSource;
            }
            return builder.ConnectionString;
        }

        protected new OleDbConnectionStringBuilder ConnectionStringBuilder =>
            (OleDbConnectionStringBuilder)base.ConnectionStringBuilder;
        public string FilePath => ConnectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);
        protected abstract byte[] EmptyDatabase { get; }

        protected AccessDatabase(DatabaseProvider provider, string connectionString)
            : base(provider, connectionString) { }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        protected override void CreateCore()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream stream = File.Open(FilePath, FileMode.CreateNew, FileAccess.Write))
            {
                stream.Write(EmptyDatabase, 0, EmptyDatabase.Length);
            }
        }

        protected override void DeleteCore()
        {
            File.Delete(FilePath);
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
