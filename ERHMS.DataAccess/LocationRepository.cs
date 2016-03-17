using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class LocationRepository : TableEntityRepository<Location>
    {
        public LocationRepository(IDataDriver driver)
            : base(driver, "ERHMS_Locations")
        { }
    }
}
