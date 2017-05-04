using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class ControlPageNavigationBehavior : Behavior<UIElement>
    {
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register(
            "Selector",
            typeof(Selector),
            typeof(ControlPageNavigationBehavior));

        private static int Mod(int dividend, int divisor)
        {
            int result = dividend % divisor;
            return result < 0 ? result + divisor : result;
        }

        private static int GetIncrement(Key key)
        {
            switch (key)
            {
                case Key.PageUp:
                    return -1;
                case Key.PageDown:
                    return 1;
                default:
                    return 0;
            }
        }

        public Selector Selector
        {
            get { return (Selector)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers != ModifierKeys.Control)
            {
                return;
            }
            if (e.Key != Key.PageUp && e.Key != Key.PageDown)
            {
                return;
            }
            int index = Selector.SelectedIndex;
            int increment = GetIncrement(e.Key);
            while (true)
            {
                index = Mod(index + increment, Selector.Items.Count);
                if (index == Selector.SelectedIndex)
                {
                    break;
                }
                else if (((UIElement)Selector.Items[index]).IsEnabled)
                {
                    Selector.SelectedIndex = index;
                    break;
                }
            }
            e.Handled = true;
        }
    }
}
