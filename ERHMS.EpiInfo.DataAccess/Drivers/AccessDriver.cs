using System.Data.OleDb;

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

        private AccessDriver(OleDbConnectionStringBuilder builder)
            : base(DataProvider.Access, builder)
        { }

        public override string GetParameterName(int index)
        {
            return "?";
        }
    }
}
