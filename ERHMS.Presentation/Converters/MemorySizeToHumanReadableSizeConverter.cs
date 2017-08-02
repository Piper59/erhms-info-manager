using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class MemorySizeToHumanReadableSizeConverter : IValueConverter
    {
        private static readonly IList<long> Sizes = new long[]
        {
            1L << 10,
            1L << 20,
            1L << 30
        };
        private static readonly IList<string> Units = new string[]
        {
            "KB",
            "MB",
            "GB"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long size = (long)value;
            for (int index = Units.Count - 1; index >= 0; index--)
            {
                if (size > Sizes[index])
                {
                    return string.Format("{0:n1} {1}", (double)size / Sizes[index], Units[index]);
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
