using ERHMS.Common.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ERHMS.Desktop.Converters
{
    public class PathTrimmingConverter : IMultiValueConverter
    {
        public TextBlock TextBlock { get; }

        public PathTrimmingConverter(TextBlock textBlock)
        {
            TextBlock = textBlock;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(values[0] is string path))
            {
                return DependencyProperty.UnsetValue;
            }
            if (!TextBlock.IsArrangeValid)
            {
                return path;
            }
            double width = (double)values[1];
            Typeface typeface = new Typeface(
                TextBlock.FontFamily,
                TextBlock.FontStyle,
                TextBlock.FontWeight,
                TextBlock.FontStretch);
            IEnumerable<string> texts = PathExtensions.Compact(path).ToList();
            foreach (string text in texts)
            {
                FormattedText formattedText = new FormattedText(
                    text,
                    culture,
                    TextBlock.FlowDirection,
                    typeface,
                    TextBlock.FontSize,
                    TextBlock.Foreground);
                if (formattedText.Width <= width)
                {
                    return text;
                }
            }
            return texts.Last();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
