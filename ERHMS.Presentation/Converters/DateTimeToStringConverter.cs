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
            DateTime? date = (DateTime?)value;
            if (date.HasValue)
            {
                return date.Value.ToString(Format);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date;
            if (DateTime.TryParse((string)value, out date))
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
