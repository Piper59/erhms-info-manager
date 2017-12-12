using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class MemorySizeToHumanReadableSizeConverter : IValueConverter
    {
        private static readonly IDictionary<long, string> Units = new Dictionary<long, string>
        {
            { 1L << 10, "KB" },
            { 1L << 20, "MB" },
            { 1L << 30, "GB" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long)value;
            foreach (KeyValuePair<long, string> unit in Units.OrderByDescending(unit => unit.Key))
            {
                if (size > unit.Key)
                {
                    return string.Format("{0:n1} {1}", (double)size / unit.Key, unit.Value);
                }
            }
            return string.Format("{0} B", size);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
