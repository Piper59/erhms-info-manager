using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class RosterRepository : TableEntityRepository<Roster>
    {
        public RosterRepository(IDataDriver driver)
            : base(driver, "ERHMS_Rosters")
        { }
    }
}
