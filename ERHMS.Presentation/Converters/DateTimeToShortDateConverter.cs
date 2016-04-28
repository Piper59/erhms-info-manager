using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class DateTimeToShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? date = (DateTime?)value;
            if (date.HasValue)
            {
                return date.Value.ToShortDateString();
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            else
            {
                DateTime date;
                if (DateTime.TryParse(value.ToString(), out date))
                {
                    return date;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
