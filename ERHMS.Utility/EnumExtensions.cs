using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetValues<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        private static string GetDescription(FieldInfo field)
        {
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? null : attribute.Description;
        }

        public static string ToDescription(Enum value)
        {
            return GetDescription(value.GetType().GetField(value.ToString()));
        }

        public static TEnum FromDescription<TEnum>(string description)
        {
            return (TEnum)typeof(TEnum).GetFields()
                .Single(field => GetDescription(field) == description)
                .GetValue(null);
        }
    }
}
