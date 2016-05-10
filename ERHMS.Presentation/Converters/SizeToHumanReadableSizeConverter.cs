using System;
using System.Globalization;
using System.Windows.Data;

namespace ERHMS.Presentation.Converters
{
    public class SizeToHumanReadableSizeConverter : IValueConverter
    {
        private const long BytesPerKilobyte = 1024L;
        private const long BytesPerMegabyte = 1048576L;
        private const long BytesPerGigabyte = 1073741824L;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (long)value;
            if (size < BytesPerKilobyte)
            {
                return string.Format("{0} B", size);
            }
            double humanReadableSize;
            string unit;
            if (size < BytesPerMegabyte)
            {
                humanReadableSize = size / BytesPerKilobyte;
                unit = "KB";
            }
            else if (size < BytesPerGigabyte)
            {
                humanReadableSize = size / BytesPerMegabyte;
                unit = "MB";
            }
            else
            {
                humanReadableSize = size / BytesPerGigabyte;
                unit = "GB";
            }
            return string.Format("{0:n1} {1}", humanReadableSize, unit);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
