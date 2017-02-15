using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        private static MethodInfo GetMethod(Type type, bool instance, string name, Type[] argTypes)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | (instance ? BindingFlags.Instance : BindingFlags.Static);
            return type.GetMethod(name, flags, null, argTypes ?? Type.EmptyTypes, null);
        }

        public static object Invoke(Type type, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return GetMethod(type, false, methodName, argTypes).Invoke(null, args);
        }

        public static object Invoke(object obj, Type type, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return GetMethod(type, true, methodName, argTypes).Invoke(obj, args);
        }

        public static object Invoke(object obj, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return Invoke(obj, obj.GetType(), methodName, argTypes, args);
        }
    }
}
