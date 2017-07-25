using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class CanvasRepository : LinkedEntityRepository<Canvas, CanvasLink>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Canvas))
            {
                TableName = "metaCanvases"
            };
            typeMap.Get(nameof(Canvas.CanvasId)).SetId();
            typeMap.Get(nameof(Canvas.New)).SetComputed();
            typeMap.Get(nameof(Canvas.Link)).SetComputed();
            typeMap.Get(nameof(Canvas.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Canvas), typeMap);
        }

        public CanvasRepository(DataContext context)
            : base(context) { }
    }
}
