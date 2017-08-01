using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    // https://stackoverflow.com/questions/20036898/avalondock-now-loses-alt-key-adornments
    public class FixKeyboardCuesBehavior : Behavior<UIElement>
    {
        private static readonly DependencyProperty ShowKeyboardCuesProperty;

        static FixKeyboardCuesBehavior()
        {
            FieldInfo field = typeof(KeyboardNavigation).GetField("ShowKeyboardCuesProperty", BindingFlags.NonPublic | BindingFlags.Static);
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

        protected override void OnDetaching()
        {
            base.OnDetaching();
            BindingOperations.ClearBinding(AssociatedObject, ShowKeyboardCuesProperty);
        }
    }
}
