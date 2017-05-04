using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Controls
{
    public partial class RemovableItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ItemContentTemplateProperty = DependencyProperty.Register(
            "ItemContentTemplate",
            typeof(DataTemplate),
            typeof(RemovableItemsControl));
        public static readonly DependencyProperty RemoveCommandProperty = DependencyProperty.Register(
            "RemoveCommand",
            typeof(ICommand),
            typeof(RemovableItemsControl));

        public DataTemplate ItemContentTemplate
        {
            get { return (DataTemplate)GetValue(ItemContentTemplateProperty); }
            set { SetValue(ItemContentTemplateProperty, value); }
        }

        public ICommand RemoveCommand
        {
            get { return (ICommand)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        public RemovableItemsControl()
        {
            InitializeComponent();
        }
    }
}
