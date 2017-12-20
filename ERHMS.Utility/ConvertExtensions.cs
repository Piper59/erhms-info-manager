using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ERHMS.Utility
{
    public static class ConvertExtensions
    {
        public static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static IFormatter GetFormatter()
        {
            return new BinaryFormatter();
        }

        public static string ToBase64String(object value)
        {
            byte[] data;
            if (value == null)
            {
                data = new byte[] { };
            }
            else
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    GetFormatter().Serialize(stream, value);
                    data = stream.ToArray();
                }
            }
            return Convert.ToBase64String(data);
        }

        public static object FromBase64String(string value)
        {
            byte[] data = Convert.FromBase64String(value);
            if (data.Length == 0)
            {
                return null;
            }
            else
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    return GetFormatter().Deserialize(stream);
                }
            }
        }

        public static Guid? ToNullableGuid(string value)
        {
            return string.IsNullOrEmpty(value) ? (Guid?)null : Guid.Parse(value);
        }
    }
}
