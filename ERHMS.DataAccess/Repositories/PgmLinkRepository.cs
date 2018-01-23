using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : IncidentEntityRepository<PgmLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(PgmLink))
            {
                TableName = "ERHMS_PgmLinks"
            };
            typeMap.Get(nameof(PgmLink.New)).SetComputed();
            typeMap.Get(nameof(PgmLink.Id)).SetComputed();
            typeMap.Get(nameof(PgmLink.Guid)).SetComputed();
            typeMap.Get(nameof(PgmLink.PgmLinkId)).SetId();
            typeMap.Get(nameof(PgmLink.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(PgmLink), typeMap);
        }

        public PgmLinkRepository(DataContext context)
            : base(context) { }

        public void DeleteByPgmId(int pgmId)
        {
            string clauses = "WHERE [PgmId] = @PgmId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@PgmId", pgmId);
            Delete(clauses, parameters);
        }
    }
}
