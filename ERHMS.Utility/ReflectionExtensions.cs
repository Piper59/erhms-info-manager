using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class ReflectionExtensions
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public static object GetValue(object obj, Type type, string fieldName)
        {
            return type.GetField(fieldName, Flags).GetValue(obj);
        }

        public static object GetValue(object obj, string fieldName)
        {
            return GetValue(obj, obj.GetType(), fieldName);
        }

        public static object Invoke(object obj, Type type, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return type.GetMethod(methodName, Flags, null, argTypes ?? Type.EmptyTypes, null).Invoke(obj, args);
        }

        public static object Invoke(object obj, string methodName, Type[] argTypes = null, object[] args = null)
        {
            return Invoke(obj, obj.GetType(), methodName, argTypes, args);
        }
    }
}
