using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class AssignmentRepository : TableEntityRepository<Assignment>
    {
        public AssignmentRepository(IDataDriver driver)
            : base(driver, "ERHMS_Assignments")
        { }

        public IEnumerable<Assignment> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }
    }
}
