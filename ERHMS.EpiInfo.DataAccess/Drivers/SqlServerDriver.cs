using System.Data.SqlClient;

namespace ERHMS.EpiInfo.DataAccess
{
    public class SqlServerDriver : DataDriverBase
    {
        public static SqlServerDriver Create(string dataSource, string initialCatalog, string userId = null, string password = null)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = dataSource;
            builder.InitialCatalog = initialCatalog;
            if (userId == null && password == null)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                if (userId != null)
                {
                    builder.UserID = userId;
                }
                if (password != null)
                {
                    builder.Password = password;
                }
            }
            return new SqlServerDriver(builder);
        }

        public static SqlServerDriver Create(Project project)
        {
            return new SqlServerDriver(new SqlConnectionStringBuilder(project.CollectedDataConnectionString));
        }

        private SqlServerDriver(SqlConnectionStringBuilder builder)
            : base(DataProvider.SqlServer, builder)
        { }

        public override string GetParameterName(int index)
        {
            return string.Format("@p{0}", index);
        }
    }
}
