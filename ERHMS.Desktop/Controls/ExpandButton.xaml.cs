using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class ExpandButton : UserControl
    {
        public static readonly DependencyProperty AccessTextProperty = DependencyProperty.Register(
            nameof(AccessText),
            typeof(string),
            typeof(ExpandButton));

        public static readonly DependencyProperty AltTextProperty = DependencyProperty.Register(
            nameof(AltText),
            typeof(string),
            typeof(ExpandButton));

        public static readonly DependencyProperty ExpandedProperty = DependencyProperty.Register(
            nameof(Expanded),
            typeof(bool),
            typeof(ExpandButton),
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

        public ExpandButton()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Expanded = !Expanded;
        }
    }
}
