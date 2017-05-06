using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobLocationRepository : TableEntityRepository<JobLocation>
    {
        public JobLocationRepository(IDataDriver driver)
            : base(driver, "ERHMS_JobLocations") { }

        public IEnumerable<JobLocation> SelectByJobId(string jobId)
        {
            return Select("[JobId] = {@}", jobId);
        }
    }
}
