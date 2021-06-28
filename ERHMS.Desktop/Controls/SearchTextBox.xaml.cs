using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class SearchTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty = TextBox.TextProperty.AddOwner(
            typeof(SearchTextBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public SearchTextBox()
        {
            InitializeComponent();
            Thickness margin = Button.Margin;
            margin.Left *= -1;
            Button.Margin = margin;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
        }
    }
}
