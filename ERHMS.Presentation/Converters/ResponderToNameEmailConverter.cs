using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ResponderToNameEmailConverter : IValueConverter
    {
        private static readonly ResponderToNameConverter ResponderToNameConverter = new ResponderToNameConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Responder responder = value as Responder;
            if (responder == null)
            {
                return null;
            }
            else if (string.IsNullOrWhiteSpace(responder.EmailAddress))
            {
                return ResponderToNameConverter.Convert(responder);
            }
            else
            {
                return string.Format("{0} ({1})", ResponderToNameConverter.Convert(responder), responder.EmailAddress);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
