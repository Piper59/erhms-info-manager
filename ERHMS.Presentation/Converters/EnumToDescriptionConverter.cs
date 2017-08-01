using ERHMS.Utility;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class EnumToDescriptionConverter<TEnum> : IValueConverter
        where TEnum : struct
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumExtensions.ToDescription((Enum)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EnumExtensions.FromDescription<TEnum>((string)value);
        }
    }
}
