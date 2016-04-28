using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : TableEntityRepository<PgmLink>
    {
        public PgmLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_PgmLinks")
        { }

        public IEnumerable<PgmLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }
    }
}
