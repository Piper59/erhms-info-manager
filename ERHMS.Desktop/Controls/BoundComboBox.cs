using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;

namespace ERHMS.Desktop.Controls
{
    public class BoundComboBox : ComboBox
    {
        private BindingBase binding;
        public BindingBase Binding
        {
            get
            {
                return binding;
            }
            set
            {
                binding = value;
                ItemContainerStyle = GetItemContainerStyle();
                ItemTemplate = GetItemTemplate();
            }
        }

        private Style GetItemContainerStyle()
        {
            Style style = new Style(typeof(ComboBoxItem), (Style)FindResource(typeof(ComboBoxItem)));
            if (Binding != null)
            {
                style.Setters.Add(new Setter(AutomationProperties.NameProperty, Binding));
            }
            return style;
        }

        private DataTemplate GetItemTemplate()
        {
            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
            if (Binding != null)
            {
                factory.SetBinding(TextBlock.TextProperty, Binding);
            }
            return new DataTemplate
            {
                VisualTree = factory
            };
        }
    }
}
