using ERHMS.Desktop.Properties;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class StringLocalizingConverter : IValueConverter
    {
        public string Format { get; set; } = "{0}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = (string)parameter ?? Format;
            string resourceName = string.Format(format, value ?? "NULL");
            return Strings.ResourceManager.GetString(resourceName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
