using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ERHMS.Utility
{
    public static class EnumExtensions
    {
        public static string ToDescription(Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute.Description;
        }

        public static TEnum FromDescription<TEnum>(string description)
        {
            return (TEnum)typeof(TEnum).GetFields()
                .Single(field => field.GetCustomAttribute<DescriptionAttribute>().Description == description)
                .GetValue(null);
        }
    }
}
