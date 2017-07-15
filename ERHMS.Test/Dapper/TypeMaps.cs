using Dapper;
using ERHMS.Dapper;

namespace ERHMS.Test.Dapper
{
    public static class TypeMaps
    {
        public static void Initialize()
        {
            {
                TypeMap typeMap = new TypeMap(typeof(Constant))
                {
                    TableName = "Global"
                };
                typeMap.Set(nameof(Constant.ConstantId), "Identity").SetId().SetComputed();
                SqlMapper.SetTypeMap(typeof(Constant), typeMap);
            }
            {
                TypeMap typeMap = new TypeMap(typeof(Gender));
                typeMap.Get(nameof(Gender.GenderId)).SetId();
                SqlMapper.SetTypeMap(typeof(Gender), typeMap);
            }
            {
                TypeMap typeMap = new TypeMap(typeof(Person));
                typeMap.Get(nameof(Person.PersonId)).SetId();
                typeMap.Get(nameof(Person.Gender)).SetComputed();
                typeMap.Get(nameof(Person.Bmi)).SetComputed();
                SqlMapper.SetTypeMap(typeof(Person), typeMap);
            }
        }
    }
}
