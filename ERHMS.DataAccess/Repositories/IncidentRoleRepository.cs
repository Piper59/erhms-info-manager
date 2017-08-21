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
            typeMap.Get(nameof(IncidentRole.IsInUse)).SetComputed();
            typeMap.Get(nameof(IncidentRole.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(IncidentRole), typeMap);
        }

        public new DataContext Context { get; private set; }

        public IncidentRoleRepository(DataContext context)
            : base(context)
        {
            Context = context;
        }

        protected override SqlBuilder GetSelectSql()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable("ERHMS_IncidentRoles");
            sql.SelectClauses.Add(@"(
                    SELECT COUNT(*)
                    FROM [ERHMS_TeamResponders]
                    WHERE [ERHMS_TeamResponders].[IncidentRoleId] = [ERHMS_IncidentRoles].[IncidentRoleId]
                ) + (
                    SELECT COUNT(*)
                    FROM [ERHMS_JobResponders]
                    WHERE [ERHMS_JobResponders].[IncidentRoleId] = [ERHMS_IncidentRoles].[IncidentRoleId]
                ) AS [IsInUse]");
            sql.AddSeparator();
            sql.AddTable(new JoinInfo(JoinType.Inner, "ERHMS_Incidents", "IncidentId", "ERHMS_IncidentRoles"));
            return sql;
        }

        public void InsertAll(string incidentId)
        {
            foreach (Role role in Context.Roles.Select())
            {
                Save(new IncidentRole
                {
                    IncidentId = incidentId,
                    Name = role.Name
                });
            }
        }
    }
}
