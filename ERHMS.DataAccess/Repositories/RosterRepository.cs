using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class RosterRepository : EntityRepository<Roster>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Roster))
            {
                TableName = "ERHMS_Rosters"
            };
            typeMap.Get(nameof(Roster.RosterId)).SetId();
            typeMap.Get(nameof(Roster.New)).SetComputed();
            typeMap.Get(nameof(Roster.Responder)).SetComputed();
            typeMap.Get(nameof(Roster.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Roster), typeMap);
        }

        public DataContext Context { get; private set; }

        public RosterRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        public override IEnumerable<Roster> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = new SqlBuilder();
                sql.AddTable("ERHMS_Rosters");
                sql.AddSeparator();
                foreach (string tableName in Context.Responders.TableNames)
                {
                    sql.AddTableSelectClause(tableName);
                    sql.FromClauses.Add(string.Format("INNER JOIN {0} ON [ERHMS_Rosters].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName)));
                }
                sql.AddSeparator();
                sql.AddTable(JoinType.Inner, "ERHMS_Incidents", "ERHMS_Rosters", "IncidentId");
                sql.OtherClauses = clauses;
                Func<Roster, Responder, Incident, Roster> map = (roster, responder, incident) =>
                {
                    roster.Responder = responder;
                    roster.Incident = incident;
                    return roster;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<Roster> SelectUndeleted()
        {
            return Select(string.Format(
                "WHERE [ERHMS_Incidents].[Deleted] = 0 AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName)));
        }

        public override Roster SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Rosters].[RosterId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        private IEnumerable<Roster> SelectByIncidentIdInternal(string clauses, string incidentId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Roster> SelectByIncidentId(string incidentId)
        {
            return SelectByIncidentIdInternal("WHERE [ERHMS_Rosters].[IncidentId] = @IncidentId", incidentId);
        }

        public IEnumerable<Roster> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Rosters].[IncidentId] = @IncidentId AND {0}.[RECSTATUS] <> 0",
                Escape(Context.Responders.View.TableName));
            return SelectByIncidentIdInternal(clauses, incidentId);
        }
    }
}
