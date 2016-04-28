using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : TableEntityRepository<ViewLink>
    {
        public ViewLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_ViewLinks")
        { }

        public IEnumerable<ViewLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }
    }
}
