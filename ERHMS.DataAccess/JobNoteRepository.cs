using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class JobNoteRepository : TableEntityRepository<JobNote>
    {
        public JobNoteRepository(IDataDriver driver)
            : base(driver, "ERHMS_JobNotes") { }

        public IEnumerable<JobNote> SelectByJobId(string jobId)
        {
            return Select("[JobId] = {@}", jobId);
        }
    }
}
