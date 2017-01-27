using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        private static MethodInfo GetMethod(Type type, bool instance, string methodName, Type[] parameterTypes)
        {
            BindingFlags bindingFlags =
                (instance ? BindingFlags.Instance : BindingFlags.Static) |
                BindingFlags.Public |
                BindingFlags.NonPublic;
            return type.GetMethod(methodName, bindingFlags, null, parameterTypes, null);
        }

        public static object Invoke(Type type, string methodName, Type[] parameterTypes, object[] parameters)
        {
            return GetMethod(type, false, methodName, parameterTypes).Invoke(null, parameters);
        }

        public static object Invoke(object obj, Type type, string methodName, Type[] parameterTypes, object[] parameters)
        {
            return GetMethod(type, true, methodName, parameterTypes).Invoke(obj, parameters);
        }

        public static object Invoke(object obj, string methodName, Type[] parameterTypes, object[] parameters)
        {
            return Invoke(obj, obj.GetType(), methodName, parameterTypes, parameters);
        }
    }
}
