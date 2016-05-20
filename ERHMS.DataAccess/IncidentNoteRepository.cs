using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentNoteRepository : TableEntityRepository<IncidentNote>
    {
        public IncidentNoteRepository(IDataDriver driver)
            : base(driver, "ERHMS_IncidentNotes") { }

        public IEnumerable<IncidentNote> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }
    }
}
