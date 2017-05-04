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
            Format = "G";
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
            DateTime dateTime;
            if (DateTime.TryParse((string)value, out dateTime))
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
