using Dapper;
using ERHMS.Dapper;

namespace ERHMS.Test
{
    public class GenderRepository
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Gender));
            typeMap.Get(nameof(Gender.GenderId)).SetId();
            SqlMapper.SetTypeMap(typeof(Gender), typeMap);
        }
    }
}
