using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public static class ButtonExtensions
    {
        public static readonly DependencyProperty OpenContextMenuOnClick = DependencyProperty.RegisterAttached(
            "OpenContextMenuOnClick",
            typeof(bool),
            typeof(ButtonExtensions),
            new PropertyMetadata(OpenContextMenuOnClick_PropertyChanged));
        public static bool GetOpenContextMenuOnClick(Button target)
        {
            return (bool)target.GetValue(OpenContextMenuOnClick);
        }
        public static void SetOpenContextMenuOnClick(Button target, bool value)
        {
            target.SetValue(OpenContextMenuOnClick, value);
        }

        private static void OpenContextMenuOnClick_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Button target = (Button)sender;
            if (!(bool)e.OldValue && (bool)e.NewValue)
            {
                target.Click += OpenContextMenuOnClick_Target_Click;
            }
            else if ((bool)e.OldValue && !(bool)e.NewValue)
            {
                target.Click -= OpenContextMenuOnClick_Target_Click;
            }
        }

        private static void OpenContextMenuOnClick_Target_Click(object sender, RoutedEventArgs e)
        {
            Button target = (Button)sender;
            if (target.ContextMenu != null)
            {
                target.ContextMenu.DataContext = target.DataContext;
                target.ContextMenu.IsOpen = true;
            }
        }
    }
}
