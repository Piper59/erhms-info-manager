using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class RosterToResponderNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Roster roster = (Roster)value;
//            dynamic responder = App.MainProject.Responders.GetById(roster.ResponderId);
            //return string.Format("{0}, {1}", responder.LastName, responder.FirstName);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
