using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Controls
{
    public class BoundComboBox : ComboBox
    {
        private BindingBase itemText;
        public BindingBase ItemText
        {
            get
            {
                return itemText;
            }
            set
            {
                itemText = value;
                OnItemTextChanged();
            }
        }

        private void OnItemTextChanged()
        {
            ItemContainerStyle = GetItemContainerStyle();
            ItemTemplate = GetItemTemplate();
        }

        private Style GetItemContainerStyle()
        {
            Style style = new Style(typeof(ComboBoxItem), (Style)FindResource(typeof(ComboBoxItem)));
            if (ItemText != null)
            {
                style.Setters.Add(new Setter(AutomationProperties.NameProperty, ItemText));
            }
            return style;
        }

        private DataTemplate GetItemTemplate()
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            if (ItemText != null)
            {
                factory.SetBinding(TextBlock.TextProperty, ItemText);
            }
            return new DataTemplate
            {
                VisualTree = factory
            };
        }
    }
}
