using Epi;
using Epi.Data;
using ERHMS.Utility;
using System.Data.Common;

namespace ERHMS.EpiInfo
{
    public class ProjectBase : Project
    {
        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        protected ProjectBase(string driver, DbConnectionStringBuilder builder)
        {
            Log.Current.DebugFormat("Creating project: {0}", builder.ToSafeString());
            CollectedDataDriver = driver;
            CollectedDataConnectionString = builder.ConnectionString;
            CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = builder.ConnectionString;
            CollectedData.Initialize(CollectedDataDbInfo, driver, false);
            MetadataSource = MetadataSource.SameDb;
            Metadata.AttachDbDriver(CollectedData.GetDbDriver());
        }
    }
}
