using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class PathToDirectoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetDirectoryName((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
