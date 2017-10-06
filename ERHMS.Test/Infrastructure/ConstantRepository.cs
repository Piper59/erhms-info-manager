using Dapper;
using ERHMS.Dapper;

namespace ERHMS.Test
{
    public class ConstantRepository
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Constant))
            {
                TableName = "Global"
            };
            typeMap.Set(nameof(Constant.ConstantId), "Identity").SetId().SetComputed();
            SqlMapper.SetTypeMap(typeof(Constant), typeMap);
        }
    }
}
