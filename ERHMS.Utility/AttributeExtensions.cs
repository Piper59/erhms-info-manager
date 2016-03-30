using System.Linq;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class AttributeExtensions
    {
        public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider @this, bool inherit = false)
        {
            return @this.GetCustomAttributes(inherit)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}
