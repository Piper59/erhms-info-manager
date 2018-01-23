using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class ControlPageNavigationBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register(
            "Selector",
            typeof(Selector),
            typeof(ControlPageNavigationBehavior));

        private static int Mod(int dividend, int divisor)
        {
            int remainder = dividend % divisor;
            return remainder < 0 ? remainder + divisor : remainder;
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

        private Window window;

        public Selector Selector
        {
            get { return (Selector)GetValue(SelectorProperty); }
            set { SetValue(SelectorProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
            if (AssociatedObject.IsLoaded)
            {
                Attach();
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            Detach();
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            Attach();
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            Detach();
        }

        private void Attach()
        {
            window = Window.GetWindow(AssociatedObject);
            window.PreviewKeyDown += Window_PreviewKeyDown;
        }

        private new void Detach()
        {
            window.PreviewKeyDown -= Window_PreviewKeyDown;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers != ModifierKeys.Control
                || (e.Key != Key.PageUp && e.Key != Key.PageDown)
                || !Selector.IsVisible)
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
                UIElement element = Selector.Items[index] as UIElement;
                if (element != null && element.IsEnabled)
                {
                    Selector.SelectedIndex = index;
                    break;
                }
            }
            e.Handled = true;
        }
    }
}
