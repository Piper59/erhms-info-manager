using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class AttributeExtensions
    {
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this ICustomAttributeProvider @this)
        {
            return @this.GetCustomAttributes(false).OfType<TAttribute>();
        }

        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider @this)
        {
            return @this.GetCustomAttributes<TAttribute>().SingleOrDefault();
        }

        public static bool HasCustomAttribute<TAttribute>(this ICustomAttributeProvider @this)
        {
            return @this.GetCustomAttributes<TAttribute>().Any();
        }
    }
}
