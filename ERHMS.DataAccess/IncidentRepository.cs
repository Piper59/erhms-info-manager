using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentRepository : TableEntityRepository<Incident>
    {
        public IncidentRepository(IDataDriver driver)
            : base(driver, "ERHMS_Incidents") { }

        public override Incident Create()
        {
            Incident incident = base.Create();
            incident.Phase = Phase.PreDeployment;
            incident.Deleted = false;
            return incident;
        }

        public IEnumerable<Incident> SelectUndeleted()
        {
            return Select("[Deleted] = 0");
        }
    }
}
