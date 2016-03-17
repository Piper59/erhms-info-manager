using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public ResponderRepository(IDataDriver driver)
            : base(driver, "Responders")
        { }

        public override Responder Create()
        {
            Responder responder = base.Create();
            responder.IsVolunteer = false;
            return responder;
        }
    }
}
