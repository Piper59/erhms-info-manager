using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class RegistrationRepository : TableEntityRepository<Registration>
    {
        public RegistrationRepository(IDataDriver driver)
            : base(driver, "ERHMS_Registrations")
        { }
    }
}
