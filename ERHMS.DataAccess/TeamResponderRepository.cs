using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class TeamResponderRepository : TableEntityRepository<TeamResponder>
    {
        public TeamResponderRepository(IDataDriver driver)
            : base(driver, "ERHMS_TeamResponders") { }

        public IEnumerable<TeamResponder> SelectByTeamId(string teamId)
        {
            return Select("[TeamId] = {@}", teamId);
        }
    }
}
