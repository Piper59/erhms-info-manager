using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class ResponsiveFormGrid : Grid
    {
        public ResponsiveFormGrid()
        {
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.0, GridUnitType.Auto)
            });
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1.0, GridUnitType.Star)
            });
        }
    }
}
