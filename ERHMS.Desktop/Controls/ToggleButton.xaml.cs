using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class ToggleButton : UserControl
    {
        public static readonly DependencyProperty AccessTextProperty = DependencyProperty.Register(
            nameof(AccessText),
            typeof(string),
            typeof(ToggleButton));

        public static readonly DependencyProperty AltTextProperty = DependencyProperty.Register(
            nameof(AltText),
            typeof(string),
            typeof(ToggleButton));

        public static readonly DependencyProperty ExpandedProperty = DependencyProperty.Register(
            nameof(Expanded),
            typeof(bool),
            typeof(ToggleButton),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string AccessText
        {
            get { return (string)GetValue(AccessTextProperty); }
            set { SetValue(AccessTextProperty, value); }
        }

        public string AltText
        {
            get { return (string)GetValue(AltTextProperty); }
            set { SetValue(AltTextProperty, value); }
        }

        public bool Expanded
        {
            get { return (bool)GetValue(ExpandedProperty); }
            set { SetValue(ExpandedProperty, value); }
        }

        public ToggleButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Expanded = !Expanded;
        }
    }
}
