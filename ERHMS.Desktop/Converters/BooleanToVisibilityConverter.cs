using System.Windows;

namespace ERHMS.Desktop.Converters
{
    public class BooleanToVisibilityConverter : ConditionalConverter
    {
        public BooleanToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }
    }
}
