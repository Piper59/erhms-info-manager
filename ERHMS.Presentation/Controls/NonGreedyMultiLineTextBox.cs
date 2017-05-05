using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class NonGreedyMultiLineTextBox : TextBox
    {
        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Height = MinHeight;
            return base.MeasureOverride(constraint);
        }
    }
}
