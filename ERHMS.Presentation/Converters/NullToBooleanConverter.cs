using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? new object() : null;
        }
    }
}
