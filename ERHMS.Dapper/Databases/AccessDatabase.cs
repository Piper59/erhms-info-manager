using ERHMS.Utility;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace ERHMS.Dapper
{
    public class AccessDatabase : Database
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

        private OleDbConnectionStringBuilder builder;
        public override DbConnectionStringBuilder Builder
        {
            get { return builder; }
        }

        public override string Name
        {
            get { return Path.GetFileNameWithoutExtension(builder.DataSource); }
        }

        public AccessDatabase(OleDbConnectionStringBuilder builder)
        {
            this.builder = builder;
        }

        public AccessDatabase(string connectionString)
            : this(new OleDbConnectionStringBuilder(connectionString)) { }

        public override bool Exists()
        {
            return File.Exists(builder.DataSource);
        }

        public override void Create()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(builder.DataSource));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Dapper.Databases.Empty.mdb", builder.DataSource);
        }

        protected override IDbConnection GetConnectionInternal()
        {
            return new OleDbConnection(builder.ConnectionString);
        }
    }
}
