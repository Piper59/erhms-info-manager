using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class ClearableTextBox : UserControl
    {
        public static DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(ClearableTextBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ClearableTextBox()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Thickness padding = TextBox.Padding;
            padding.Right += Clear.ActualWidth;
            TextBox.Padding = padding;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
            GetBindingExpression(TextProperty).UpdateSource();
        }
    }
}
