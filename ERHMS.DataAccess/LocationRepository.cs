using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class LocationRepository : TableEntityRepository<Location>
    {
        public LocationRepository(IDataDriver driver)
            : base(driver, "ERHMS_Locations") { }

        public IEnumerable<Location> SelectByIncidentId(string incidentId)
        {
            return Select("[IncidentId] = {@}", incidentId);
        }
    }
}
