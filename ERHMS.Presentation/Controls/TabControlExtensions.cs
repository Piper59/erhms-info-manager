using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Controls
{
    public static class TabControlExtensions
    {
        public static readonly DependencyProperty ControlPageNavigationTabControlProperty = DependencyProperty.RegisterAttached(
            "ControlPageNavigationTabControl",
            typeof(TabControl),
            typeof(TabControlExtensions),
            new PropertyMetadata(ControlPageNavigationTabControl_PropertyChanged));
        public static TabControl GetControlPageNavigationTabControl(UIElement target)
        {
            return (TabControl)target.GetValue(ControlPageNavigationTabControlProperty);
        }
        public static void SetControlPageNavigationTabControl(UIElement target, TabControl value)
        {
            target.SetValue(ControlPageNavigationTabControlProperty, value);
        }

        private static void ControlPageNavigationTabControl_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement target = (UIElement)sender;
            if (e.OldValue != null)
            {
                target.PreviewKeyDown -= ControlPageNavigationTabControl_Target_PreviewKeyDown;
            }
            if (e.NewValue != null)
            {
                target.PreviewKeyDown += ControlPageNavigationTabControl_Target_PreviewKeyDown;
            }
        }

        private static void ControlPageNavigationTabControl_Target_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.PageUp || e.Key == Key.PageDown) &&
                (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                UIElement target = (UIElement)sender;
                TabControl tabControl = GetControlPageNavigationTabControl(target);
                int selectedIndex = tabControl.SelectedIndex + (e.Key == Key.PageUp ? -1 : 1);
                if (selectedIndex < 0)
                {
                    selectedIndex = tabControl.Items.Count - 1;
                }
                else if (selectedIndex >= tabControl.Items.Count)
                {
                    selectedIndex = 0;
                }
                tabControl.SelectedIndex = selectedIndex;
                e.Handled = true;
            }
        }
    }
}
