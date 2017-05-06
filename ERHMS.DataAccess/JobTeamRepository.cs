using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobTeamRepository : TableEntityRepository<JobTeam>
    {
        public JobTeamRepository(IDataDriver driver)
            : base(driver, "ERHMS_JobTeams") { }

        public IEnumerable<JobTeam> SelectByJobId(string jobId)
        {
            return Select("[JobId] = {@}", jobId);
        }
    }
}
