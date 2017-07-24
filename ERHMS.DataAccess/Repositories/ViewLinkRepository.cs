using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : LinkRepository<ViewLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(ViewLink))
            {
                TableName = "ERHMS_ViewLinks"
            };
            typeMap.Get(nameof(ViewLink.ViewLinkId)).SetId();
            typeMap.Get(nameof(ViewLink.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(ViewLink), typeMap);
        }

        public ViewLinkRepository(DataContext context)
            : base(context) { }

        public void DeleteByViewId(int viewId)
        {
            string clauses = "WHERE [ViewId] = @ViewId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ViewId", viewId);
            Delete(clauses, parameters);
        }
    }
}
