using Epi;
using ERHMS.EpiInfo.DataAccess;
using System.IO;

namespace ERHMS.Presentation.ViewModels
{
    public class DataSourceViewModel : ViewModelBase
    {
        public class SqlServerDataSourceViewModel : ViewModelBase
        {
            private string dataSource;
            public string DataSource
            {
                get { return dataSource; }
                set { Set(() => DataSource, ref dataSource, value); }
            }

            private string initialCatalog;
            public string InitialCatalog
            {
                get { return initialCatalog; }
                set { Set(() => InitialCatalog, ref initialCatalog, value); }
            }

            private bool integratedSecurity;
            public bool IntegratedSecurity
            {
                get { return integratedSecurity; }
                set { Set(() => IntegratedSecurity, ref integratedSecurity, value); }
            }

            private string userId;
            public string UserId
            {
                get { return userId; }
                set { Set(() => UserId, ref userId, value); }
            }

            private string password;
            public string Password
            {
                get { return password; }
                set { Set(() => Password, ref password, value); }
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { Set(() => Name, ref name, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { Set(() => Description, ref description, value); }
        }

        private DirectoryInfo location;
        public DirectoryInfo Location
        {
            get { return location; }
            set { Set(() => Location, ref location, value); }
        }

        private DataProvider provider;
        public DataProvider Provider
        {
            get { return provider; }
            set { Set(() => Provider, ref provider, value); }
        }

        public SqlServerDataSourceViewModel SqlServer { get; private set; }

        public DataSourceViewModel()
        {
            SqlServer = new SqlServerDataSourceViewModel();
        }

        public void Reset()
        {
            Configuration configuration = Configuration.GetNewInstance();
            Name = null;
            Description = null;
            Location = new DirectoryInfo(configuration.Directories.Project);
            Provider = DataProvider.Access;
            SqlServer.DataSource = null;
            SqlServer.InitialCatalog = null;
            SqlServer.IntegratedSecurity = true;
            SqlServer.UserId = null;
            SqlServer.Password = null;
        }
    }
}
