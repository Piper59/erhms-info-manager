using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public ResponderRepository(IDataDriver driver, Project project)
            : base(driver, project.Views["Responders"]) { }

        public override Responder Create()
        {
            Responder responder = base.Create();
            responder.IsVolunteer = false;
            return responder;
        }
    }
}
