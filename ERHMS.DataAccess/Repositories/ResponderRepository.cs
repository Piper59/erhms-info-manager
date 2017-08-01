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

        public new DataContext Context { get; private set; }

        public ResponderRepository(DataContext context)
            : base(context, context.Project.Views["Responders"])
        {
            Context = context;
        }

        public IEnumerable<Responder> SelectUnrostered(string incidentId)
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
    }
}
