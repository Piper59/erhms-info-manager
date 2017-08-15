using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Responder));
            SqlMapper.SetTypeMap(typeof(Responder), typeMap);
        }

        public ResponderRepository(DataContext context)
            : base(context, context.Project.Views["Responders"]) { }

        public IEnumerable<Responder> SelectRosterable(string incidentId)
        {
            string format = @"
                WHERE {0}.[RECSTATUS] <> 0
                AND {0}.[GlobalRecordId] NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectTeamable(string incidentId, string teamId)
        {
            string format = @"
                WHERE {0}.[RECSTATUS] <> 0
                AND {0}.[GlobalRecordId] IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.[GlobalRecordId] NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_TeamResponders]
                    WHERE [TeamId] = @TeamId
                )";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add(@"TeamId", teamId);
            return Select(clauses, parameters);
        }
    }
}
