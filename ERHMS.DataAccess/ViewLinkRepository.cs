using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : TableEntityRepository<ViewLink>
    {
        public ViewLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_ViewLinks")
        { }
    }
}
