using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERHMS.Common.Reflection
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetInstanceTypes(this Type @this, Assembly assembly)
        {
            return assembly.ExportedTypes.Where(type => @this.IsAssignableFrom(type) && !type.IsAbstract);
        }

        public static IEnumerable<Type> GetInstanceTypes(this Type @this)
        {
            return @this.GetInstanceTypes(@this.Assembly);
        }

        public static Stream GetManifestResourceStream(this Type @this, string resourceName)
        {
            return @this.Assembly.GetManifestResourceStream(@this, resourceName);
        }
    }
}
