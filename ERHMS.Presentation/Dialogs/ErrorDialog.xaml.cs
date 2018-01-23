using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Presentation.Dialogs
{
    public partial class ErrorDialog : CustomDialog
    {
        private static MetroDialogSettings GetSettings()
        {
            return new MetroDialogSettings
            {
                CustomResourceDictionary = (ResourceDictionary)Application.Current.Resources["Accent"]
            };
        }

        public static async Task ShowAsync(MetroWindow window, string message, Exception exception)
        {
            ErrorDialog dialog = new ErrorDialog(window, message, exception);
            await dialog.ShowAsync();
        }

        public ErrorDialog(MetroWindow window, string message, Exception exception)
            : base(window, GetSettings())
        {
            InitializeComponent();
            Title = "Error";
            Message.Text = message;
            Exception.Text = string.Format("{0}: {1}", exception.GetType().FullName, exception.Message);
        }

        private async void OK_Click(object sender, RoutedEventArgs e)
        {
            await OwningWindow.HideMetroDialogAsync(this);
        }

        public async Task ShowAsync()
        {
            await OwningWindow.ShowMetroDialogAsync(this);
            await WaitUntilUnloadedAsync();
        }
    }
}
