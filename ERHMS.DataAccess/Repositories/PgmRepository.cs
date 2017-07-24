using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class PgmRepository : LinkedEntityRepository<Pgm, PgmLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Pgm))
            {
                TableName = "metaPrograms"
            };
            typeMap.Get(nameof(Pgm.PgmId)).SetId();
            typeMap.Get(nameof(Pgm.PgmLink)).SetComputed();
            typeMap.Get(nameof(Pgm.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Pgm), typeMap);
        }

        public PgmRepository(DataContext context)
            : base(context) { }

        protected override void SetLink(Pgm entity, PgmLink link)
        {
            entity.PgmLink = link;
        }
    }
}
