using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public new DataContext Context { get; private set; }

        public RosterRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        public override IEnumerable<Roster> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                ICollection<string> tableNames = Context.Responders.TableNames.ToList();
                StringBuilder format = new StringBuilder();
                format.AppendFormat(
                    "SELECT [ERHMS_Rosters].*, NULL AS [Separator1], [ERHMS_Incidents].*, NULL AS [Separator2], {0}",
                    string.Join(", ", tableNames.Select(tableName => string.Format("{0}.*", Escape(tableName)))));
                format.AppendFormat(" FROM {0}", new string('(', tableNames.Count));
                format.Append("[ERHMS_Rosters] INNER JOIN [ERHMS_Incidents] ON [ERHMS_Rosters].[IncidentId] = [ERHMS_Incidents].[IncidentId]");
                foreach (string tableName in tableNames)
                {
                    format.AppendFormat(") INNER JOIN {0} ON [ERHMS_Rosters].[ResponderId] = {0}.[GlobalRecordId]", Escape(tableName));
                }
                format.Append(" {0}");
                string sql = string.Format(format.ToString(), clauses);
                Func<Roster, Incident, Responder, Roster> map = (roster, incident, responder) =>
                {
                    roster.New = false;
                    incident.New = false;
                    responder.New = false;
                    roster.Incident = incident;
                    roster.Responder = responder;
                    return roster;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator1, Separator2");
            });
        }

        public override Roster SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Rosters].[RosterId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<Roster> SelectByIncidentId(string incidentId)
        {
            string clauses = "WHERE [ERHMS_Rosters].[IncidentId] = @IncidentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }
    }
}
