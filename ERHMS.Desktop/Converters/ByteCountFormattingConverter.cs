using ERHMS.Common.Text;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class ByteCountFormattingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IFormatProvider formatter = new ByteCountFormatter();
            string format = (string)parameter ?? "{0:N0}";
            return string.Format(formatter, format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
