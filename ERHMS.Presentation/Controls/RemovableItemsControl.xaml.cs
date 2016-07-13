using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ERHMS.Presentation.Controls
{
    public partial class RemovableItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ItemTextTemplateProperty = DependencyProperty.Register(
            "ItemTextTemplate",
            typeof(DataTemplate),
            typeof(RemovableItemsControl));
        public DataTemplate ItemTextTemplate
        {
            get { return (DataTemplate)GetValue(ItemTextTemplateProperty); }
            set { SetValue(ItemTextTemplateProperty, value); }
        }

        public RemovableItemsControl()
        {
            InitializeComponent();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                FrameworkElement source = (FrameworkElement)e.Source;
                FrameworkElement parent = (FrameworkElement)source.TemplatedParent;
                Button button = (Button)Root.ItemTemplate.FindName("Remove", parent);
                ICommand command = (ICommand)button.GetValue(ButtonBase.CommandProperty);
                if (command != null && command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }
}
