using ERHMS.Common;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class StringCaseConverter : IValueConverter
    {
        public IValueConverter BaseConverter { get; set; }
        public string Format { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (BaseConverter != null)
            {
                value = BaseConverter.Convert(value, targetType, parameter, culture);
            }
            IFormatProvider formatProvider = new StringCaseFormatter
            {
                Culture = culture
            };
            return string.Format(formatProvider, Format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
