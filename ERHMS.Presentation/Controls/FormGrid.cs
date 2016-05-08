using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class FormGrid : Grid
    {
        public static readonly GridLength LabelWidth = new GridLength(150.0);

        public FormGrid()
        {
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = LabelWidth
            });
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.0, GridUnitType.Star)
            });
        }
    }
}
