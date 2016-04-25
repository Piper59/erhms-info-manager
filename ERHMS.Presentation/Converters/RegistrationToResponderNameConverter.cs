using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class RegistrationToResponderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Registration registration = (Registration)value;
//            dynamic responder = App.MainProject.Responders.GetById(registration.ResponderId);
            //return string.Format("{0}, {1}", responder.LastName, responder.FirstName);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
