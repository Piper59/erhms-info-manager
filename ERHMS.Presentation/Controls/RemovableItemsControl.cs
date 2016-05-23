using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.Controls
{
    public class RemovableItemsControl : ItemsControl
    {
        public static readonly DependencyProperty ItemTextTemplateProperty = DependencyProperty.Register(
            "ItemTextTemplate",
            typeof(DataTemplate),
            typeof(RemovableItemsControl),
            null);
        public DataTemplate ItemTextTemplate
        {
            get { return (DataTemplate)GetValue(ItemTextTemplateProperty); }
            set { SetValue(ItemTextTemplateProperty, value); }
        }
    }
}
