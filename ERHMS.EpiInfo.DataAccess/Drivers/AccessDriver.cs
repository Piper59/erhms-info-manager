using ADOX;
using System.Data.OleDb;
using System.IO;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        public static AccessDriver Create(OleDbProvider provider, string dataSource, string password = null)
        {
            OleDbConnectionStringBuilder builder = new OleDbConnectionStringBuilder();
            builder.Provider = provider.GetName();
            builder.DataSource = dataSource;
            if (password != null)
            {
                builder["Jet OLEDB:Database Password"] = password;
            }
            return new AccessDriver(builder);
        }

        private OleDbConnectionStringBuilder builder;

        private AccessDriver(OleDbConnectionStringBuilder builder)
            : base(DataProvider.Access, builder.ConnectionString)
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
            Catalog catalog = new Catalog();
            catalog.Create(builder.ConnectionString);
        }
    }
}
