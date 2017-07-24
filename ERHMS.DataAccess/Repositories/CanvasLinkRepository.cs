using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : LinkRepository<CanvasLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(CanvasLink))
            {
                TableName = "ERHMS_CanvasLinks"
            };
            typeMap.Get(nameof(CanvasLink.CanvasLinkId)).SetId();
            typeMap.Get(nameof(CanvasLink.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(CanvasLink), typeMap);
        }

        public CanvasLinkRepository(DataContext context)
            : base(context) { }

        public void DeleteByCanvasId(int canvasId)
        {
            string clauses = "WHERE [CanvasId] = @CanvasId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CanvasId", canvasId);
            Delete(clauses, parameters);
        }
    }
}
