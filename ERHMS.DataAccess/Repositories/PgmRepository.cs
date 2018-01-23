using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class PgmRepository : EpiInfoEntityRepository<Pgm, PgmLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Pgm))
            {
                TableName = "metaPrograms"
            };
            typeMap.Get(nameof(Pgm.New)).SetComputed();
            typeMap.Get(nameof(Pgm.Id)).SetComputed();
            typeMap.Set(nameof(Pgm.PgmId), ColumnNames.PROGRAM_ID).SetId();
            typeMap.Get(nameof(Pgm.Link)).SetComputed();
            typeMap.Get(nameof(Pgm.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Pgm), typeMap);
        }

        public PgmRepository(DataContext context)
            : base(context) { }
    }
}
