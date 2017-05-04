using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public RosterRepository Rosters { get; private set; }

        public ResponderRepository(IDataDriver driver, Project project, RosterRepository rosters)
            : base(driver, project.Views["Responders"])
        {
            Rosters = rosters;
        }

        public override Responder Create()
        {
            Responder responder = base.Create();
            responder.IsVolunteer = false;
            return responder;
        }

        public IEnumerable<Responder> SelectByIncidentId(string incidentId)
        {
            ICollection<string> responderIds = Rosters.SelectByIncidentId(incidentId)
                .Select(roster => roster.ResponderId)
                .ToList();
            return SelectUndeleted().Where(responder => responderIds.ContainsIgnoreCase(responder.ResponderId));
        }
    }
}
