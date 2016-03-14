using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class IncidentRepository : TableRepository<Incident>
    {
        public IncidentRepository(IDataDriver driver)
            : base(driver, "ERHMS_Incidents")
        { }

        public override Incident Create()
        {
            Incident incident = base.Create();
            incident.Phase = Phase.PreDeployment;
            return incident;
        }
    }
}
