using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobRepository : TableEntityRepository<Job>
    {
        public JobRepository(IDataDriver driver)
            : base(driver, "ERHMS_Jobs") { }

        public IEnumerable<Job> SelectByIncidentId(string incidentId)
        {
            return Select("[IncidentId] = {@}", incidentId);
        }
    }
}
