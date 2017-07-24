using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

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
            typeMap.Get(nameof(IncidentNote.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentNote), typeMap);
        }

        public IncidentNoteRepository(DataContext context)
            : base(context) { }
    }
}
