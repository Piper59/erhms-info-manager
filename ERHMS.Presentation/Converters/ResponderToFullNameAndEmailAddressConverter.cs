using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ResponderToFullNameAndEmailAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Responder responder = (Responder)value;
            return string.Format("{0} ({1})", responder.FullName, responder.EmailAddress);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
