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

        public DataContext Context { get; private set; }

        public IncidentRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        public IEnumerable<Incident> SelectUndeleted()
        {
            string clauses = "WHERE [Deleted] = 0";
            return Select(clauses);
        }

        public IEnumerable<Incident> SelectUndeletedByResponderId(string responderId)
        {
            string clauses = "WHERE [IncidentId] IN (SELECT [IncidentId] FROM [ERHMS_Rosters] WHERE [ResponderId] = @ResponderId) AND [Deleted] = 0";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public override void Insert(Incident entity)
        {
            base.Insert(entity);
            Context.IncidentRoles.InsertAll(entity.IncidentId);
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
