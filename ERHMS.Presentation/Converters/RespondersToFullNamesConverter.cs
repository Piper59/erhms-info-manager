using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class RespondersToFullNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<string> fullNames = ((IEnumerable<Responder>)value).Select(responder => responder.FullName)
                .OrderBy(fullName => fullName, StringComparer.OrdinalIgnoreCase);
            return string.Join("; ", fullNames);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
