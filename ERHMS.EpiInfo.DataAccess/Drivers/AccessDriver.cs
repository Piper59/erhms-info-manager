using ADOX;
using ERHMS.Utility;
using System.Data.OleDb;
using System.IO;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        public static AccessDriver Create(string dataSource, string password = null)
        {
            string name = Path.GetFileNameWithoutExtension(dataSource);
            DirectoryInfo location = new DirectoryInfo(Path.GetDirectoryName(dataSource));
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Provider = "Microsoft.Jet.OLEDB.4.0";
            builder.DataSource = dataSource;
            if (password != null)
            {
                builder["Jet OLEDB:Database Password"] = password;
            }
            return new AccessDriver(name, location, builder);
        }

        private OleDbConnectionStringBuilder builder;

        private AccessDriver(string name, DirectoryInfo location, OleDbConnectionStringBuilder builder)
            : base(name, location, DataProvider.Access, builder)
        {
            this.builder = builder;
        }

        public override string GetParameterName(int index)
        {
            return "?";
        }

        public override bool DatabaseExists()
        {
            return File.Exists(builder.DataSource);
        }

        public override void CreateDatabase()
        {
            Log.Current.DebugFormat("Creating database: {0}", builder.ToSafeString());
            Catalog catalog = new Catalog();
            catalog.Create(builder.ConnectionString);
        }
    }
}
