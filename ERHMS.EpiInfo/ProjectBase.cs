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
            InitializeCollectedData(driver, connectionString);
            InitializeMetadata();
        }

        private void InitializeCollectedData(string driver, string connectionString)
        {
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };
            Log.Current.DebugFormat("Opening project: {0}", string.Join(", ", builder
                .Cast<KeyValuePair<string, object>>()
                .Where(property => !property.Key.ToLower().Contains("password"))
                .Select(property => string.Format("{0} = {1}", property.Key, property.Value))));
            CollectedDataDbInfo.DBCnnStringBuilder = builder;
            CollectedData.Initialize(CollectedDataDbInfo, driver, false);
        }

        private void InitializeMetadata()
        {
            MetadataSource = MetadataSource.SameDb;
            Metadata.AttachDbDriver(CollectedData.GetDbDriver());
        }
    }
}
