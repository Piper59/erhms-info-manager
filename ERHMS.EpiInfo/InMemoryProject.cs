using Epi;
using Epi.Data.Services;

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
            collectedData.Initialize(CollectedDataDbInfo, driver, false);
            metadata = new MetadataDbProvider(this);
            metadata.AttachDbDriver(collectedData.GetDbDriver());
        }

        public override void Save() { }
    }
}
