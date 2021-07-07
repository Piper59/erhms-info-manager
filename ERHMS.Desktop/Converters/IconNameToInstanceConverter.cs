using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class IconNameToInstanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = (string)parameter ?? "{0}";
            string iconName = string.Format(format, value ?? "NULL");
            return Icons.GetInstance(iconName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
