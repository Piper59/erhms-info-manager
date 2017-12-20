using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class NonGreedyTextBox : TextBox
    {
        static NonGreedyTextBox()
        {
            HeightProperty.OverrideMetadata(typeof(NonGreedyTextBox), new FrameworkPropertyMetadata(double.NaN));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Height = MinHeight;
            return base.MeasureOverride(constraint);
        }
    }
}
