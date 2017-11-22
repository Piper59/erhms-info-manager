using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class RoleRepository : EntityRepository<Role>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Role))
            {
                TableName = "ERHMS_Roles"
            };
            typeMap.Get(nameof(Role.RoleId)).SetId();
            typeMap.Get(nameof(Role.New)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Role), typeMap);
        }

        public RoleRepository(DataContext context)
            : base(context.Database) { }
    }
}
