using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ERHMS.Desktop.Infrastructure
{
    public static class IInputElementExtensions
    {
        public static bool IsDescendantOf(this IInputElement @this, DependencyObject ancestor)
        {
            return @this is Visual visual && visual.IsDescendantOf(ancestor);
        }

        public static bool MoveFocus(this IInputElement @this, FocusNavigationDirection direction)
        {
            return @this is FrameworkElement element && element.MoveFocus(new TraversalRequest(direction));
        }
    }
}
