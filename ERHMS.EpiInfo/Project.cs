using Epi;
using ERHMS.Utility;
using System.Data.Common;

namespace ERHMS.EpiInfo
{
    public class Project : Epi.Project
    {
        public Project(string driver, DbConnectionStringBuilder builder)
        {
            Log.Current.DebugFormat("Opening project: {0}, {1}", driver, builder.ToSafeString());
            CollectedDataDriver = driver;
            CollectedDataConnectionString = builder.ConnectionString;
            CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = builder.ConnectionString;
            CollectedData.Initialize(CollectedDataDbInfo, driver, false);
            MetadataSource = MetadataSource.SameDb;
            Metadata.AttachDbDriver(CollectedData.GetDbDriver());
        }
    }
}
