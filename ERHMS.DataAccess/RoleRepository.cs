using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class RoleRepository : TableEntityRepository<Role>
    {
        public RoleRepository(IDataDriver driver)
            : base(driver, "ERHMS_Roles") { }
    }
}
