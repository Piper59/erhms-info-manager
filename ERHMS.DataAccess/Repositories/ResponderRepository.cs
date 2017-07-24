using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class ResponderRepository : ViewEntityRepository<Responder>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Responder));
            SqlMapper.SetTypeMap(typeof(Responder), typeMap);
        }

        public new DataContext Context { get; private set; }

        public ResponderRepository(DataContext context)
            : base(context, context.Project.Views["Responders"])
        {
            Context = context;
        }
    }
}
