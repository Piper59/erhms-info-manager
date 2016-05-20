using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class RosterRepository : TableEntityRepository<Roster>
    {
        public RosterRepository(IDataDriver driver)
            : base(driver, "ERHMS_Rosters") { }

        public IEnumerable<Roster> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }
    }
}
