using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class FormRepository : TableEntityRepository<Form>
    {
        public FormRepository(IDataDriver driver)
            : base(driver, "ERHMS_Forms")
        { }
    }
}
