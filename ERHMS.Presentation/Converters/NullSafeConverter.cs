using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class NullSafeConverter : IValueConverter
    {
        public IValueConverter Base { get; private set; }

        public NullSafeConverter(IValueConverter @base)
        {
            Base = @base;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : Base.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? null : Base.ConvertBack(value, targetType, parameter, culture);
        }
    }
}
