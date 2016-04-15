using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : TableEntityRepository<PgmLink>
    {
        public PgmLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_PgmLinks")
        { }
    }
}
