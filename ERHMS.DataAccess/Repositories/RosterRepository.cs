using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
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

        private SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_Rosters");
            sql.AddSeparator();
            foreach (string tableName in Context.Responders.TableNames)
            {
                sql.AddTable(JoinType.Inner, tableName, ColumnNames.GLOBAL_RECORD_ID, "ERHMS_Rosters", "ResponderId");
            }
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_Rosters.IncidentId");
            return sql;
        }

        private Roster Map(Roster roster, Responder responder, Incident incident)
        {
            roster.Responder = responder;
            roster.Incident = incident;
            return roster;
        }

        public override IEnumerable<Roster> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<Roster, Responder, Incident, Roster>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public override Roster SelectById(object id)
        {
            string clauses = "WHERE [ERHMS_Rosters].[RosterId] = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<Roster> SelectUndeletedByIncidentId(string incidentId)
        {
            string clauses = string.Format(
                "WHERE [ERHMS_Rosters].[IncidentId] = @IncidentId AND {0}.{1} <> 0",
                Escape(Context.Responders.View.TableName),
                Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }
    }
}
