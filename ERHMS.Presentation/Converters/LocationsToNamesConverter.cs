using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class LocationsToNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<string> names = ((IEnumerable<Location>)value).Select(location => location.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase);
            return string.Join(", ", names);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
