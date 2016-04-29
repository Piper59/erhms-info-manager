using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Controls
{
    public static class DataGridExtensions
    {
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.RegisterAttached(
            "DoubleClickCommand",
            typeof(ICommand),
            typeof(DataGridExtensions),
            new FrameworkPropertyMetadata(Target_PropertyChanged));
        public static ICommand GetDoubleClickCommand(DataGrid target)
        {
            return (ICommand)target.GetValue(DoubleClickCommandProperty);
        }
        public static void SetDoubleClickCommand(DataGrid target, ICommand value)
        {
            target.SetValue(DoubleClickCommandProperty, value);
        }

        private static void Target_PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid target = (DataGrid)sender;
            if (e.OldValue == null && e.NewValue != null)
            {
                target.MouseDoubleClick += Target_MouseDoubleClick;
            }
            else if (e.OldValue != null && e.NewValue == null)
            {
                target.MouseDoubleClick -= Target_MouseDoubleClick;
            }
        }

        private static void Target_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid target = (DataGrid)sender;
            ICommand command = GetDoubleClickCommand(target);
            if (command == null)
            {
                return;
            }
            if (command.CanExecute(target))
            {
                command.Execute(target);
            }
        }
    }
}
