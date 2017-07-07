using System;
using System.Reflection;

namespace ERHMS.Utility
{
    public class Invoker
    {
        private Type declaringType;
        private Type[] argTypes;

        public object Object { get; set; }

        public Type DeclaringType
        {
            get { return declaringType ?? Object?.GetType(); }
            set { declaringType = value; }
        }

        public string MethodName { get; set; }

        private BindingFlags BindingFlags
        {
            get { return BindingFlags.Public | BindingFlags.NonPublic | (Object == null ? BindingFlags.Static : BindingFlags.Instance); }
        }

        public Type[] ArgTypes
        {
            get { return argTypes ?? Type.EmptyTypes; }
            set { argTypes = value; }
        }

        public object Invoke(params object[] args)
        {
            return DeclaringType.GetMethod(MethodName, BindingFlags, null, ArgTypes, null).Invoke(Object, args);
        }
    }
}
