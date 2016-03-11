using Epi;
using Epi.Data.Services;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public class InMemoryProject : Project
    {
        public InMemoryProject(string connectionString, string driver)
            : base()
        {
            CollectedDataConnectionString = connectionString;
            CollectedDataDriver = driver;
            MetadataSource = MetadataSource.SameDb;
            CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = connectionString;
            Log.Current.DebugFormat("Opening project: {0}", string.Join(", ", CollectedDataDbInfo.DBCnnStringBuilder
                .Cast<KeyValuePair<string, object>>()
                .Where(pair => !pair.Key.ToLower().Contains("password"))
                .Select(pair => string.Format("{0} = {1}", pair.Key, pair.Value))));
            collectedData.Initialize(CollectedDataDbInfo, driver, false);
            metadata = new MetadataDbProvider(this);
            metadata.AttachDbDriver(collectedData.GetDbDriver());
        }

        public override void Save() { }
    }
}
