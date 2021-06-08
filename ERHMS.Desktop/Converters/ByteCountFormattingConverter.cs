using ERHMS.Common.Text;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class ByteCountFormattingConverter : IValueConverter
    {
        public string Format { get; set; } = "{0:N0}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IFormatProvider formatter = new ByteCountFormatter();
            return string.Format(formatter, Format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
