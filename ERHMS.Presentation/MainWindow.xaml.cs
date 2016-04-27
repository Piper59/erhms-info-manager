using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;

namespace ERHMS.Presentation
{
    public partial class MainWindow : MetroWindow
    {
        private bool closing;

        public MainWindow()
        {
            Closing += MainWindow_Closing;
            Messenger.Default.Register<ExitMessage>(this, OnExitMessage);
            InitializeComponent();
        }

        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (closing)
            {
                return;
            }
            e.Cancel = true;
            MessageDialogResult result = await this.ShowMessageAsync(
                "Exit?",
                string.Format("Are you sure you want to exit {0}?", App.Title),
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Exit",
                    NegativeButtonText = "Don't Exit",
                    AnimateHide = false
                });
            if (result == MessageDialogResult.Affirmative)
            {
                closing = true;
                Close();
                closing = false;
            }
        }

        private void OnExitMessage(ExitMessage msg)
        {
            Close();
        }
    }
}
