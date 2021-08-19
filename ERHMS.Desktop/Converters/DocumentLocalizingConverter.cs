using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Markdown;
using ERHMS.Desktop.Properties;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace ERHMS.Desktop.Converters
{
    public class DocumentLocalizingConverter : IValueConverter
    {
        private static readonly RenderingContext context = new RenderingContext
        {
            Heading1StyleKey = "Heading1",
            Heading2StyleKey = "Heading2",
            Heading3StyleKey = "Heading3",
            EmphasisStyleKey = "Emphasis"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string format = (string)parameter ?? "{0}";
            string resourceName = string.Format(format, value ?? "NULL");
            object obj = Strings.ResourceManager.GetObject(resourceName);
            if (obj is byte[] data)
            {
                string text = Encoding.UTF8.GetString(data);
                FlowDocument document = new FlowDocument();
                document.Blocks.AddRange(context.GetBlocks(text));
                return document;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
