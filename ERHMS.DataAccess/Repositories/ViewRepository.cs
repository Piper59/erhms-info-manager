using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class ViewRepository : LinkedEntityRepository<View, ViewLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(View))
            {
                TableName = "metaViews"
            };
            typeMap.Get(nameof(View.ViewId)).SetId();
            typeMap.Get(nameof(View.ViewLink)).SetComputed();
            typeMap.Get(nameof(View.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(View), typeMap);
        }

        public ViewRepository(DataContext context)
            : base(context) { }

        protected override void SetLink(View entity, ViewLink link)
        {
            entity.ViewLink = link;
        }
    }
}
