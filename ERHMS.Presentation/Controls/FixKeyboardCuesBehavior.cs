using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Controls
{
    public class FixKeyboardCuesBehavior : Behavior<UIElement>
    {
        private static readonly DependencyProperty ShowKeyboardCuesProperty;

        static FixKeyboardCuesBehavior()
        {
            FieldInfo field = typeof(KeyboardNavigation).GetField("ShowKeyboardCuesProperty", BindingFlags.Static | BindingFlags.NonPublic);
            ShowKeyboardCuesProperty = (DependencyProperty)field.GetValue(null);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Binding binding = new Binding("(KeyboardNavigation.ShowKeyboardCues)")
            {
                Source = Window.GetWindow(AssociatedObject)
            };
            BindingOperations.SetBinding(AssociatedObject, ShowKeyboardCuesProperty, binding);
        }
    }
}
