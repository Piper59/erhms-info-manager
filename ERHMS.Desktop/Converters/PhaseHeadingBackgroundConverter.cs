using ERHMS.Desktop.Infrastructure;
using ERHMS.Domain;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ERHMS.Desktop.Converters
{
    public class PhaseHeadingBackgroundConverter : IValueConverter
    {
        private const byte StartAlpha = 0x40;
        private const byte EndAlpha = 0x00;
        private static readonly Point StartPoint = new Point(0.5, 0.0);
        private static readonly Point EndPoint = new Point(1.0, 0.0);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = ((Phase)value).ToColor();
            Color startColor = color.CopyWithAlpha(StartAlpha);
            Color endColor = color.CopyWithAlpha(EndAlpha);
            return new LinearGradientBrush(startColor, endColor, StartPoint, EndPoint);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
