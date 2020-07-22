using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ERHMS.Desktop.Converters
{
    public class IconToImageSourceConverter : IValueConverter
    {
        private const int DefaultSize = 32;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            int size = parameter == null ? DefaultSize : (int)parameter;
            return Imaging.CreateBitmapSourceFromHIcon(
                ((Icon)value).Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(size, size));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
