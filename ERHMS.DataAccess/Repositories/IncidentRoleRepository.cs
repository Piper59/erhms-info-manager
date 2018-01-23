using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class IncidentRoleRepository : IncidentEntityRepository<IncidentRole>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(IncidentRole))
            {
                TableName = "ERHMS_IncidentRoles"
            };
            typeMap.Get(nameof(IncidentRole.New)).SetComputed();
            typeMap.Get(nameof(IncidentRole.Id)).SetComputed();
            typeMap.Get(nameof(IncidentRole.Guid)).SetComputed();
            typeMap.Get(nameof(IncidentRole.IncidentRoleId)).SetId();
            typeMap.Get(nameof(IncidentRole.InUse)).SetComputed();
            typeMap.Get(nameof(IncidentRole.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentRole), typeMap);
        }

        public DataContext Context { get; private set; }

        public IncidentRoleRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        protected override SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_IncidentRoles");
            sql.SelectClauses.Add(@"
                (
                    SELECT COUNT(*)
                    FROM [ERHMS_TeamResponders]
                    WHERE [ERHMS_TeamResponders].[IncidentRoleId] = [ERHMS_IncidentRoles].[IncidentRoleId]
                ) + (
                    SELECT COUNT(*)
                    FROM [ERHMS_JobResponders]
                    WHERE [ERHMS_JobResponders].[IncidentRoleId] = [ERHMS_IncidentRoles].[IncidentRoleId]
                ) AS [InUse]");
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents.IncidentId", "ERHMS_IncidentRoles.IncidentId");
            return sql;
        }

        public void InsertAll(string incidentId)
        {
            foreach (Role role in Context.Roles.Select())
            {
                Insert(new IncidentRole(true)
                {
                    IncidentId = incidentId,
                    Name = role.Name
                });
            }
        }
    }
}
