using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ERHMS.Desktop.Converters
{
    public class ActionTextBoxPaddingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness padding = (Thickness)values[0];
            double buttonWidth = (double)values[1];
            padding.Right += buttonWidth;
            return padding;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
