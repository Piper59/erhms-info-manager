using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ResponderToFullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Responder)value).FullName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
