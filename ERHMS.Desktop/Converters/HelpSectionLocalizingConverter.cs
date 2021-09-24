using ERHMS.Desktop.Markdown;
using ERHMS.Desktop.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace ERHMS.Desktop.Converters
{
    public class HelpSectionLocalizingConverter : IValueConverter
    {
        public RenderingContext Context { get; } = new RenderingContext
        {
            Heading1StyleKey = "Heading1",
            Heading2StyleKey = "Heading2",
            Heading3StyleKey = "Heading3",
            EmphasizedStyleKey = "Emphasized",
            StrongStyleKey = "Strong",
            CodeStyleKey = "Code"
        };

        private Block GetTitle(object value)
        {
            string title = Strings.ResourceManager.GetString($"Help.Title.{value}");
            Run run = new Run(title);
            Paragraph paragraph = new Paragraph(run);
            paragraph.SetResourceReference(FrameworkElement.StyleProperty, Context.Heading1StyleKey);
            return paragraph;
        }

        private IEnumerable<Block> GetBody(object value)
        {
            object obj = Strings.ResourceManager.GetObject($"Help.Body.{value}");
            if (obj is byte[] data)
            {
                string text = Encoding.UTF8.GetString(data);
                return Context.GetBlocks(text);
            }
            else
            {
                return Enumerable.Empty<Block>();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FlowDocument document = new FlowDocument();
            document.Blocks.Add(GetTitle(value));
            document.Blocks.AddRange(GetBody(value));
            return document;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
