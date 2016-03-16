using Epi;
using Epi.Data;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public class ProjectBase : Project
    {
        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        protected ProjectBase(string driver, string connectionString)
        {
            CollectedDataDriver = driver;
            CollectedDataConnectionString = connectionString;
            InitializeCollectedData();
            InitializeMetadata();
        }

        private void InitializeCollectedData()
        {
            DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder
            {
                ConnectionString = CollectedDataConnectionString
            };
            Log.Current.DebugFormat("Opening project: {0}", string.Join(", ", connectionStringBuilder
                .Cast<KeyValuePair<string, object>>()
                .Where(connectionProperty => !connectionProperty.Key.ToLower().Contains("password"))
                .Select(connectionProperty => string.Format("{0} = {1}", connectionProperty.Key, connectionProperty.Value))));
            CollectedDataDbInfo.DBCnnStringBuilder = connectionStringBuilder;
            CollectedData.Initialize(CollectedDataDbInfo, CollectedDataDriver, false);
        }

        private void InitializeMetadata()
        {
            MetadataSource = MetadataSource.SameDb;
            Metadata.AttachDbDriver(CollectedData.GetDbDriver());
        }
    }
}
