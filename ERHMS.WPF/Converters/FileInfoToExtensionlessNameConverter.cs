using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace ERHMS.WPF.Converters
{
    public class FileInfoToExtensionlessNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetFileNameWithoutExtension(((FileInfo)value).Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
