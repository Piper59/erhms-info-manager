using ERHMS.Utility;
using System.Data.OleDb;
using System.IO;
using System.Reflection;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        public static AccessDriver Create(string dataSource, string password = null)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            builder.DataSource = dataSource;
            if (password != null)
            {
                builder["Jet OLEDB:Database Password"] = password;
            }
            return new AccessDriver(builder);
        }

        public static AccessDriver Create(Project project)
        {
            return new AccessDriver(new OleDbConnectionStringBuilder(project.CollectedDataConnectionString));
        }

        public new OleDbConnectionStringBuilder Builder { get; private set; }

        public AccessDriver(OleDbConnectionStringBuilder builder)
            : base(DataProvider.Access, builder, Path.GetFileNameWithoutExtension(builder.DataSource))
        {
            Builder = builder;
        }

        public override string GetParameterName(int index)
        {
            return "?";
        }

        public override bool DatabaseExists()
        {
            return File.Exists(Builder.DataSource);
        }

        public override void CreateDatabase()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Builder.DataSource));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.EpiInfo.DataAccess.Drivers.Empty.mdb", Builder.DataSource);
        }

        public override bool TableExists(string tableName)
        {
            try
            {
                string sql = string.Format("SELECT TOP 1 * FROM {0}", Escape(tableName));
                ExecuteQuery(sql);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
