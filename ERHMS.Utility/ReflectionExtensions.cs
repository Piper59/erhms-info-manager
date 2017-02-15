using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        public static object Invoke(object obj, Type type, string methodName, Type[] argTypes = null, object[] args = null)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return type.GetMethod(methodName, flags, null, argTypes ?? Type.EmptyTypes, null).Invoke(obj, args);
        }

        public static object Invoke(object obj, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return Invoke(obj, obj.GetType(), methodName, argTypes, args);
        }
    }
}
