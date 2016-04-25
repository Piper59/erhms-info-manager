using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class IncidentNoteRepository : TableEntityRepository<IncidentNote>
    {
        public IncidentNoteRepository(IDataDriver driver)
            : base(driver, "ERHMS_IncidentNotes")
        { }
    }
}
