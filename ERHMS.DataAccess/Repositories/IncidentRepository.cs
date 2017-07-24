using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentRepository : EntityRepository<Incident>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Incident))
            {
                TableName = "ERHMS_Incidents"
            };
            typeMap.Get(nameof(Incident.IncidentId)).SetId();
            SqlMapper.SetTypeMap(typeof(Incident), typeMap);
        }

        public new DataContext Context { get; private set; }

        public IncidentRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public IEnumerable<Incident> SelectUndeleted()
        {
            return Select("WHERE [Deleted] = 0");
        }
    }
}
