using Epi;
using System.Linq;
using System.Reflection;

namespace ERHMS.Data
{
    public enum DatabaseType
    {
        [Driver(Configuration.AccessDriver)]
        Access,

        [Driver(Configuration.SqlDriver)]
        SqlServer
    }

    public static class DatabaseTypeExtensions
    {
        public static string ToDriver(this DatabaseType @this)
        {
            return typeof(DatabaseType).GetField(@this.ToString())
                .GetCustomAttribute<DriverAttribute>()
                .Driver;
        }

        public static DatabaseType FromDriver(string driver)
        {
            object value = typeof(DatabaseType).GetFields()
                .Single(field => field.GetCustomAttribute<DriverAttribute>().Driver == driver)
                .GetValue(null);
            return (DatabaseType)value;
        }
    }
}
