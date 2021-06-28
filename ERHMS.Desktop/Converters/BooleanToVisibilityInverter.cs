using System.Windows;

namespace ERHMS.Desktop.Converters
{
    public class BooleanToVisibilityInverter : ConditionalConverter
    {
        public BooleanToVisibilityInverter()
        {
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
        }
    }
}
