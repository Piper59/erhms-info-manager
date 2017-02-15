using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static string ToBase64String(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static object FromBase64String(string value)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(value)))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(stream);
            }
        }

        public static Guid? ToNullableGuid(string value)
        {
            return value == null ? (Guid?)null : Guid.Parse(value);
        }
    }
}
