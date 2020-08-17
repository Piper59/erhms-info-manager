using ERHMS.Desktop.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DialogView : Window
    {
        private static DialogButton GetDataContext(Button button)
        {
            return (DialogButton)button.DataContext;
        }

        public DialogView()
        {
            InitializeComponent();
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            if (GetDataContext(button).IsDefault)
            {
                button.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool? result = GetDataContext((Button)e.Source).Result;
            if (DialogResult == result)
            {
                Close();
            }
            else
            {
                DialogResult = result;
            }
        }
    }
}
