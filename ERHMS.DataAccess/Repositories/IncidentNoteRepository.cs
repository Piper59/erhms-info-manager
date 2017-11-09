using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class IncidentNoteRepository : LinkRepository<IncidentNote>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(IncidentNote))
            {
                TableName = "ERHMS_IncidentNotes"
            };
            typeMap.Get(nameof(IncidentNote.IncidentNoteId)).SetId();
            typeMap.Get(nameof(IncidentNote.New)).SetComputed();
            typeMap.Get(nameof(IncidentNote.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentNote), typeMap);
        }

        public IncidentNoteRepository(DataContext context)
            : base(context) { }

        public IEnumerable<IncidentNote> SelectByIncidentIdAndDateRange(string incidentId, DateTime? start, DateTime? end)
        {
            ICollection<string> clauses = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            clauses.Add("[ERHMS_IncidentNotes].[IncidentId] = @IncidentId");
            parameters.Add("@IncidentId", incidentId);
            if (start.HasValue)
            {
                clauses.Add("[ERHMS_IncidentNotes].[Date] >= @Start");
                parameters.Add("@Start", start.Value.RemoveMilliseconds());
            }
            if (end.HasValue)
            {
                clauses.Add("[ERHMS_IncidentNotes].[Date] <= @End");
                parameters.Add("@End", end.Value.RemoveMilliseconds());
            }
            return Select(string.Format("WHERE {0}", string.Join(" AND ", clauses)), parameters);
        }
    }
}
