using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DialogView : Window
    {
        public new DialogViewModel DataContext
        {
            get { return (DialogViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public DialogView()
        {
            InitializeComponent();
            Loaded += DialogView_Loaded;
        }

        private void DialogView_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext.Severity.ToSound()?.Play();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.Source;
            DialogButton dialogButton = (DialogButton)button.DataContext;
            DialogResult = dialogButton.Result;
            Close();
        }
    }
}
