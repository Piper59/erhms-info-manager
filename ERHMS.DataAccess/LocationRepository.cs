using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class LocationRepository : TableRepository<Location>
    {
        public LocationRepository(IDataDriver driver)
            : base(driver, "ERHMS_Locations")
        { }
    }
}
