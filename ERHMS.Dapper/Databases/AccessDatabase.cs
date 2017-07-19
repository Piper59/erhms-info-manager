using ERHMS.Utility;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace ERHMS.Dapper
{
    public class AccessDatabase : DatabaseBase
    {
        public static AccessDatabase Construct(string dataSource, string password = null)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Provider = OleDbExtensions.Providers.Jet4;
            builder.DataSource = dataSource;
            if (password != null)
            {
                builder["Jet OLEDB:Database Password"] = password;
            }
            return new AccessDatabase(builder);
        }

        public OleDbConnectionStringBuilder Builder { get; private set; }

        public AccessDatabase(OleDbConnectionStringBuilder builder)
        {
            Builder = builder;
        }

        public AccessDatabase(string connectionString)
            : this(new OleDbConnectionStringBuilder(connectionString)) { }

        public override bool Exists()
        {
            return File.Exists(Builder.DataSource);
        }

        public override void Create()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Builder.DataSource));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Dapper.Databases.Empty.mdb", Builder.DataSource);
        }

        protected override IDbConnection GetConnectionInternal()
        {
            return new OleDbConnection(Builder.ConnectionString);
        }
    }
}
