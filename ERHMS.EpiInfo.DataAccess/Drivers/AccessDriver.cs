using ADOX;
using System.Data.OleDb;
using System.IO;
using System.Threading;

namespace ERHMS.EpiInfo.DataAccess
{
    public class AccessDriver : DataDriverBase
    {
        private const int CreationAttemptCount = 10;
        private const int CreationAttemptDelay = 1000;

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

        private AccessDriver(OleDbConnectionStringBuilder builder)
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
            // TODO: Copy an embedded resource instead?
            Directory.CreateDirectory(Path.GetDirectoryName(Builder.DataSource));
            Catalog catalog = new Catalog();
            catalog.Create(Builder.ConnectionString);
            for (int attempt = 1; attempt <= CreationAttemptCount; attempt++)
            {
                if (attempt > 1)
                {
                    Thread.Sleep(CreationAttemptDelay);
                }
                try
                {
                    using (OleDbConnection connection = new OleDbConnection(Builder.ConnectionString))
                    {
                        connection.Open();
                    }
                    return;
                }
                catch { }
            }
        }
    }
}
