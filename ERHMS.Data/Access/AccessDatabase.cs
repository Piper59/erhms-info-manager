using System.Data.OleDb;
using System.IO;

namespace ERHMS.Data.Access
{
    public abstract class AccessDatabase : Database
    {
        protected new OleDbConnectionStringBuilder ConnectionStringBuilder =>
            (OleDbConnectionStringBuilder)base.ConnectionStringBuilder;
        public string FilePath => ConnectionStringBuilder.DataSource;
        public override string Name => Path.GetFileNameWithoutExtension(FilePath);

        protected AccessDatabase(DatabaseProvider provider, string connectionString)
            : base(provider, connectionString)
        {
            CommandBuilder.QuotePrefix = "[";
            CommandBuilder.QuoteSuffix = "]";
        }

        public override bool Exists()
        {
            return File.Exists(FilePath);
        }

        protected abstract void CreateCore(Stream stream);

        protected override void CreateCore()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream stream = File.Open(FilePath, FileMode.CreateNew, FileAccess.Write))
            {
                CreateCore(stream);
            }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}
