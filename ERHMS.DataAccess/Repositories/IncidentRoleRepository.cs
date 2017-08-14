using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class IncidentRoleRepository : LinkRepository<IncidentRole>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(IncidentRole))
            {
                TableName = "ERHMS_IncidentRoles"
            };
            typeMap.Get(nameof(IncidentRole.IncidentRoleId)).SetId();
            typeMap.Get(nameof(IncidentRole.New)).SetComputed();
            typeMap.Get(nameof(IncidentRole.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentRole), typeMap);
        }

        public IncidentRoleRepository(DataContext context)
            : base(context) { }
    }
}
