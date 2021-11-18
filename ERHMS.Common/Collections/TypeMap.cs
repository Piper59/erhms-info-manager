using System;
using System.Collections.Generic;

namespace ERHMS.Common.Collections
{
    [Serializable]
    public class TypeMap : Dictionary<Type, Type>
    {
        public Type Map(Type key)
        {
            while (true)
            {
                if (TryGetValue(key, out Type value))
                {
                    return value;
                }
                key = key.BaseType;
                if (key == null)
                {
                    throw new KeyNotFoundException();
                }
            }
        }
    }
}
