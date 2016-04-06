using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.WPF.Converters
{
    public class AssignmentToResponderFirstNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Assignment assignment = (Assignment)value;
//            dynamic responder = App.MainProject.Responders.GetById(assignment.ResponderId);
//            return responder.FirstName;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
