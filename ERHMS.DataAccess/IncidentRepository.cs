using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentRepository : TableEntityRepository<Incident>
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

        private DataPredicate GetDeletedPredicate(bool deleted)
        {
            DataParameter parameter;
            string sql = GetConditionalSql(Schema.Columns["Deleted"], deleted ? 1 : 0, out parameter);
            return new DataPredicate(sql, parameter);
        }

        public IEnumerable<Incident> SelectByDeleted(bool deleted)
        {
            return Select(GetDeletedPredicate(deleted));
        }

        public IEnumerable<Incident> SelectByDeleted(bool deleted, DataPredicate predicate)
        {
            return Select(GetDeletedPredicate(deleted), predicate);
        }

        public virtual IEnumerable<Incident> SelectByDeleted(bool deleted, IEnumerable<DataPredicate> predicates)
        {
            return Select(predicates.Prepend(GetDeletedPredicate(deleted)));
        }

        public virtual IEnumerable<Incident> SelectByDeleted(bool deleted, params DataPredicate[] predicates)
        {
            return Select(predicates.Prepend(GetDeletedPredicate(deleted)));
        }
    }
}
