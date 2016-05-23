using ERHMS.Presentation.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class RecipientToStringConverter : IValueConverter
    {
        private static readonly ResponderToNameConverter ResponderToNameConverter = new ResponderToNameConverter();

        public string Convert(RecipientViewModel recipient)
        {
            if (recipient.IsResponder)
            {
                if (recipient.Responder == null)
                {
                    return null;
                }
                else
                {
                    return ResponderToNameConverter.Convert(recipient.Responder);
                }
            }
            else
            {
                return recipient.EmailAddress;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((RecipientViewModel)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
