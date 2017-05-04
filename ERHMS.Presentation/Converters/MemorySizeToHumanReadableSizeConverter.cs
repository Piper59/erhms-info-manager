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
            long memorySize = (long)value;
            for (int index = Units.Count - 1; index > 0; index--)
            {
                if (memorySize > Sizes[index])
                {
                    return string.Format("{0:n1} {1}", (double)memorySize / Sizes[index], Units[index]);
                }
            }
            return string.Format("{0} B", memorySize);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
