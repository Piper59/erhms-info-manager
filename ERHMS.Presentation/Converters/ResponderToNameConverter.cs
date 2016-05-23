using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class ResponderToNameConverter : IValueConverter
    {
        public string Convert(Responder responder)
        {
            return string.Format("{0}, {1}", responder.LastName, responder.FirstName);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Responder responder = value as Responder;
            if (responder == null)
            {
                return null;
            }
            else
            {
                return Convert(responder);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
