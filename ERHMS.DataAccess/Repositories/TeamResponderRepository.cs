using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class TeamResponderRepository : EntityRepository<TeamResponder>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(TeamResponder))
            {
                TableName = "ERHMS_TeamResponders"
            };
            typeMap.Get(nameof(TeamResponder.TeamResponderId)).SetId();
            typeMap.Get(nameof(TeamResponder.New)).SetComputed();
            typeMap.Get(nameof(TeamResponder.Team)).SetComputed();
            typeMap.Get(nameof(TeamResponder.Responder)).SetComputed();
            typeMap.Get(nameof(TeamResponder.IncidentRole)).SetComputed();
            SqlMapper.SetTypeMap(typeof(TeamResponder), typeMap);
        }

        public DataContext Context { get; private set; }

        public TeamResponderRepository(DataContext context)
            : base(context.Database)
        {
            Context = context;
        }

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_TeamResponders");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Teams.TeamId", "ERHMS_TeamResponders.TeamId");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Teams.IncidentId", "ERHMS_TeamIncidents");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_TeamResponders", "ResponderId");
            }
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_IncidentRoles.IncidentRoleId", "ERHMS_TeamResponders.IncidentRoleId");
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents.IncidentId", "ERHMS_IncidentRoles.IncidentId", "ERHMS_IncidentRoleIncidents");
            return sql;
        }

        private TeamResponder Map(
            TeamResponder teamResponder,
            Team team,
            Incident teamIncident,
            Responder responder,
            IncidentRole incidentRole,
            Incident incidentRoleIncident)
        {
            teamResponder.Team = team;
            team.Incident = teamIncident;
            teamResponder.Responder = responder;
            if (incidentRole.IncidentRoleId != null)
            {
                teamResponder.IncidentRole = incidentRole;
                incidentRole.Incident = incidentRoleIncident;
            }
            return teamResponder;
        }

        public override IEnumerable<TeamResponder> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<TeamResponder, Team, Incident, Responder, IncidentRole, Incident, TeamResponder>(
                    sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override TeamResponder SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_TeamResponders].[TeamResponderId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<TeamResponder> SelectUndeletedByTeamId(string teamId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_TeamResponders].[TeamId] = @TeamId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<TeamResponder> SelectUndeletedByResponderId(string responderId)
        {
            string clauses = @"
                WHERE [ERHMS_TeamResponders].[ResponderId] = @ResponderId
                AND [ERHMS_TeamIncidents].[Deleted] = 0
                AND ([ERHMS_IncidentRoleIncidents].[Deleted] IS NULL OR [ERHMS_IncidentRoleIncidents].[Deleted] = 0)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void DeleteByTeamId(string teamId)
        {
            string clauses = "WHERE [TeamId] = @TeamId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            Delete(clauses, parameters);
        }
    }
}
