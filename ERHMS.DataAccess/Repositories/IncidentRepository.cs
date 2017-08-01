using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
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
            typeMap.Get(nameof(Incident.New)).SetComputed();
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

        public override void Delete(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public override void DeleteById(object id)
        {
            throw new NotSupportedException();
        }

        public override void Delete(Incident entity)
        {
            throw new NotSupportedException();
        }
    }
}
