using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Infrastructure
{
    public static class IInputElementExtensions
    {
        public static bool MoveFocus(this IInputElement @this, FocusNavigationDirection direction)
        {
            if (@this is FrameworkElement element)
            {
                return element.MoveFocus(new TraversalRequest(direction));
            }
            else
            {
                return false;
            }
        }
    }
}
