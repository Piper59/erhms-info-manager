using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobResponderRepository : TableEntityRepository<JobResponder>
    {
        public JobResponderRepository(IDataDriver driver)
            : base(driver, "ERHMS_JobResponder") { }

        public IEnumerable<JobResponder> SelectByJobId(string jobId)
        {
            return Select("[JobId] = {@}", jobId);
        }
    }
}
