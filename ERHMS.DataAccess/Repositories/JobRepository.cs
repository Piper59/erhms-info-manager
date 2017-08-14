using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class JobRepository : LinkRepository<Job>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Job))
            {
                TableName = "ERHMS_Jobs"
            };
            typeMap.Get(nameof(Job.JobId)).SetId();
            typeMap.Get(nameof(Job.New)).SetComputed();
            typeMap.Get(nameof(Job.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Job), typeMap);
        }

        public JobRepository(DataContext context)
            : base(context) { }
    }
}
