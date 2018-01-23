using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentNoteRepository : IncidentEntityRepository<IncidentNote>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(IncidentNote))
            {
                TableName = "ERHMS_IncidentNotes"
            };
            typeMap.Get(nameof(IncidentNote.New)).SetComputed();
            typeMap.Get(nameof(IncidentNote.Id)).SetComputed();
            typeMap.Get(nameof(IncidentNote.Guid)).SetComputed();
            typeMap.Get(nameof(IncidentNote.IncidentNoteId)).SetId();
            typeMap.Get(nameof(IncidentNote.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentNote), typeMap);
        }

        public IncidentNoteRepository(DataContext context)
            : base(context) { }

        public IEnumerable<IncidentNote> SelectByIncidentIdAndDateRange(string incidentId, DateTime? startDate, DateTime? endDate)
        {
            ICollection<string> conditions = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            conditions.Add("[ERHMS_IncidentNotes].[IncidentId] = @IncidentId");
            parameters.Add("@IncidentId", incidentId);
            if (startDate.HasValue)
            {
                conditions.Add("[ERHMS_IncidentNotes].[Date] >= @StartDate");
                parameters.Add("@StartDate", startDate.Value.RemoveMilliseconds());
            }
            if (endDate.HasValue)
            {
                conditions.Add("[ERHMS_IncidentNotes].[Date] <= @EndDate");
                parameters.Add("@EndDate", endDate.Value.RemoveMilliseconds());
            }
            return Select(SqlBuilder.GetWhereClause(conditions), parameters);
        }
    }
}
