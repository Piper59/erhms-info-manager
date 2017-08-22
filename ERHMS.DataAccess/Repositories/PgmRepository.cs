using Dapper;
using Epi;
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
            typeMap.Set(nameof(Pgm.PgmId), ColumnNames.PROGRAM_ID).SetId();
            typeMap.Get(nameof(Pgm.New)).SetComputed();
            typeMap.Get(nameof(Pgm.Link)).SetComputed();
            typeMap.Get(nameof(Pgm.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Pgm), typeMap);
        }

        protected override PropertyMap LinkPropertyMap
        {
            get { return LinkTypeMap.Get(nameof(PgmLink.PgmId)); }
        }

        public PgmRepository(DataContext context)
            : base(context) { }
    }
}
