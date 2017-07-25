using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class EnumExtensions
    {
        public static TEnum Parse<TEnum>(string value)
            where TEnum : struct
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value);
        }

        public static IEnumerable<TEnum> GetValues<TEnum>()
            where TEnum : struct
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        private static string GetDescription(FieldInfo field)
        {
            return field.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        public static string ToDescription(Enum value)
        {
            return GetDescription(value.GetType().GetField(value.ToString()));
        }

        public static TEnum FromDescription<TEnum>(string description)
            where TEnum : struct
        {
            return (TEnum)typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(TEnum))
                .Single(field => GetDescription(field) == description)
                .GetValue(null);
        }
    }
}
