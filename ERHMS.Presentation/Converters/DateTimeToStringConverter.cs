using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public string Format { get; set; }

        public DateTimeToStringConverter()
        {
            Format = "g";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? dateTime = (DateTime?)value;
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToString(Format);
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
                DateTime dateTime;
                if (DateTime.TryParse(value.ToString(), out dateTime))
                {
                    return dateTime;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
