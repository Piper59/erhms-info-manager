using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class ComboBoxHeader : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ComboBoxHeader));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ComboBoxHeader()
        {
            InitializeComponent();
        }
    }
}
