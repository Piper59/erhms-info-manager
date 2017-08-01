using ERHMS.Utility;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    public static class PrintExtensions
    {
        public static void PrintPlainText(string title, string content)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FlowDocument document = new FlowDocument
                {
                    ColumnWidth = dialog.PrintableAreaWidth,
                    PagePadding = new Thickness(48.0),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12.0
                };
                Paragraph paragraph = new Paragraph();
                foreach (string line in content.SplitLines())
                {
                    paragraph.Inlines.Add(new Run(line));
                    paragraph.Inlines.Add(new LineBreak());
                }
                document.Blocks.Add(paragraph);
                dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, title);
            }
        }
    }
}
