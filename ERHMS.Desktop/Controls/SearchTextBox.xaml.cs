using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Controls
{
    public partial class SearchTextBox : UserControl
    {
        // TODO: Does this work properly without specifying two-way binding?
        public static readonly DependencyProperty TextProperty = TextBox.TextProperty.AddOwner(typeof(SearchTextBox));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public SearchTextBox()
        {
            InitializeComponent();
            SetClearButtonMargin();
        }

        // TODO: Does this work?
        private void SetClearButtonMargin()
        {
            Thickness margin = ClearButton.Margin;
            margin.Left *= -1;
            ClearButton.Margin = margin;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
        }
    }
}
