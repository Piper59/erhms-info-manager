using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public ResponderRepository(DataContext context)
            : base(context, context.Project.Views["Responders"]) { }

        public IEnumerable<Responder> SelectByTeamId(string teamId)
        {
            string format = @"
                WHERE {0}.[GlobalRecordId] IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_TeamResponders]
                    WHERE [TeamId] = @TeamId
                )
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectRosterable(string incidentId)
        {
            string format = @"
                WHERE {0}.[GlobalRecordId] NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectTeamable(string incidentId, string teamId)
        {
            string format = @"
                WHERE {0}.[GlobalRecordId] IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.[GlobalRecordId] NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_TeamResponders]
                    WHERE [TeamId] = @TeamId
                )
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add("@TeamId", teamId);
            return Select(clauses, parameters);
        }

        public IEnumerable<Responder> SelectJobbable(string incidentId, string jobId)
        {
            string format = @"
                WHERE {0}.[GlobalRecordId] IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_Rosters]
                    WHERE [IncidentId] = @IncidentId
                )
                AND {0}.[GlobalRecordId] NOT IN (
                    SELECT [ResponderId]
                    FROM [ERHMS_JobResponders]
                    WHERE [JobId] = @JobId
                )
                AND {0}.[RECSTATUS] <> 0";
            string clauses = string.Format(format, Escape(View.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add("@JobId", jobId);
            return Select(clauses, parameters);
        }
    }
}
