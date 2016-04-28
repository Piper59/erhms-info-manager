using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class DateTimeToShortDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? date = (DateTime?)value;
            if (date.HasValue)
            {
                return string.Format("{0} {1}", date.Value.ToShortDateString(), date.Value.ToShortTimeString());
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
