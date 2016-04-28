using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ERHMS.Utility
{
    public static class EnumExtensions
    {
        public static IEnumerable<TEnum> GetValues<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static string ToDescription(Enum value)
        {
            return value.GetType()
                .GetField(value.ToString())
                .GetCustomAttribute<DescriptionAttribute>()
                .Description;
        }

        public static TEnum FromDescription<TEnum>(string description)
        {
            return (TEnum)typeof(TEnum).GetFields()
                .Single(field => field.GetCustomAttribute<DescriptionAttribute>().Description == description)
                .GetValue(null);
        }
    }
}
