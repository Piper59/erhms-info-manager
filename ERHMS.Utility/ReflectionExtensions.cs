using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        private static MethodInfo GetMethod(Type type, bool instance, string methodName, Type[] types)
        {
            BindingFlags bindingFlags = instance ? BindingFlags.Instance : BindingFlags.Static;
            bindingFlags |= BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            return type.GetMethod(methodName, bindingFlags, null, types, null);
        }

        public static object Invoke(Type type, string methodName, Type[] types, object[] parameters)
        {
            return GetMethod(type, false, methodName, types).Invoke(null, parameters);
        }

        public static object Invoke(this object @this, string methodName, Type[] types, object[] parameters)
        {
            return GetMethod(@this.GetType(), true, methodName, types).Invoke(@this, parameters);
        }
    }
}
