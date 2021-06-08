using ERHMS.Desktop.Properties;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class ResourceNameToStringConverter : IValueConverter
    {
        public string Prefix { get; set; }
        public string NullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                value = NullValue;
                if (value == null)
                {
                    return DependencyProperty.UnsetValue;
                }
            }
            string resourceName = $"{Prefix}.{value}";
            return Strings.ResourceManager.GetObject(resourceName, Strings.Culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
