using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ERHMS.Common.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInstanceTypes(this Type @this, Assembly assembly)
        {
            return assembly.GetTypes().Where(type => @this.IsAssignableFrom(type) && !type.IsAbstract);
        }

        public static IEnumerable<Type> GetInstanceTypes(this Type @this)
        {
            return @this.GetInstanceTypes(Assembly.GetCallingAssembly());
        }
    }
}
