using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class TeamRepository : LinkRepository<Team>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Team))
            {
                TableName = "ERHMS_Teams"
            };
            typeMap.Get(nameof(Team.TeamId)).SetId();
            typeMap.Get(nameof(Team.New)).SetComputed();
            typeMap.Get(nameof(Team.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Team), typeMap);
        }

        public TeamRepository(DataContext context)
            : base(context) { }
    }
}
