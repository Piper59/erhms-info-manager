using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : TableEntityRepository<CanvasLink>
    {
        public CanvasLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_CanvasLinks")
        { }
    }
}
