using Microsoft.Xaml.Behaviors;
using System.Linq;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class CopyCurrentCellToClipboard : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.CopyingRowClipboardContent += AssociatedObject_CopyingRowClipboardContent;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.CopyingRowClipboardContent -= AssociatedObject_CopyingRowClipboardContent;
        }

        private void AssociatedObject_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            DataGridClipboardCellContent currentCellContent = e.ClipboardRowContent
                .SingleOrDefault(content => content.Column.Equals(AssociatedObject.CurrentColumn));
            if (currentCellContent != null)
            {
                e.ClipboardRowContent.Clear();
                e.ClipboardRowContent.Add(currentCellContent);
            }
        }
    }
}
