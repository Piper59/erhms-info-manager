using System;
using System.Globalization;
using System.Windows.Data;
using ERHMS.Domain;

namespace ERHMS.WPF.Converters
{
    public class AssignmentToResponderLastNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Assignment assignment = (Assignment)value;
            //dynamic responder = App.MainProject.Responders.GetById(assignment.ResponderId);
            //return responder.LastName;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
