using ERHMS.Desktop.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DialogView : Window
    {
        public DialogView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            DialogButton dialogButton = (DialogButton)button.DataContext;
            bool? result = dialogButton.Result;
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
