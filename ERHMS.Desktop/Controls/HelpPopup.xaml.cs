using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class HelpPopup : UserControl
    {
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            nameof(Element),
            typeof(UIElement),
            typeof(HelpPopup));

        public UIElement Element
        {
            get { return (UIElement)GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }

        public HelpPopup()
        {
            InitializeComponent();
        }
    }
}
