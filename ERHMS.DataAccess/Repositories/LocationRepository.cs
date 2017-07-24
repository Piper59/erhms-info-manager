using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;

namespace ERHMS.DataAccess
{
    public class LocationRepository : LinkRepository<Location>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Location))
            {
                TableName = "ERHMS_Locations"
            };
            typeMap.Get(nameof(Location.LocationId)).SetId();
            typeMap.Get(nameof(Location.Incident)).SetComputed();
            SqlMapper.SetTypeMap(typeof(Location), typeMap);
        }

        public LocationRepository(DataContext context)
            : base(context) { }
    }
}
