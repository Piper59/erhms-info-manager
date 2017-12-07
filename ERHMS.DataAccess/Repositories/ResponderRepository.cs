using Dapper;
using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public static void Configure()
        {
            SqlMapper.SetTypeMap(typeof(Responder), GetTypeMap());
        }

        public ResponderRepository(DataContext context)
            : base(context.Database, context.Project.Views["Responders"]) { }

        public IEnumerable<Responder> SelectRosterable(string incidentId)
        {
            string format = @"
                WHERE {0}.{1} NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.{2} <> 0";
            string clauses = string.Format(format, Escape(View.TableName), Escape(ColumnNames.GLOBAL_RECORD_ID), Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectTeamable(string incidentId, string teamId)
        {
            string format = @"
                WHERE {0}.{1} IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.{1} NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_TeamResponders]
                    WHERE [TeamId] = @TeamId
                )
                AND {0}.{2} <> 0";
            string clauses = string.Format(format, Escape(View.TableName), Escape(ColumnNames.GLOBAL_RECORD_ID), Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add("@TeamId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectJobbable(string incidentId, string jobId)
        {
            string format = @"
                WHERE {0}.{1} IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.{1} NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_JobResponders]
                    WHERE [JobId] = @JobId
                )
                AND {0}.{2} <> 0";
            string clauses = string.Format(format, Escape(View.TableName), Escape(ColumnNames.GLOBAL_RECORD_ID), Escape(ColumnNames.REC_STATUS));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add("@JobId", jobId);
            return Select(clauses, parameters);
        }
    }
}
