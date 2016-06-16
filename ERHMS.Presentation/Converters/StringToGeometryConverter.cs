using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ERHMS.Presentation.Converters
{
    public class StringToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Geometry.Parse((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
