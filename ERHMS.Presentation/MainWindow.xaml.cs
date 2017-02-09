using ERHMS.Utility;
using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Mantin.Controls.Wpf.Notification;
using System.ComponentModel;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    // TODO: Persist window size?
    public partial class MainWindow : MetroWindow
    {
        private bool closing;

        public MainWindow(MainViewModel model)
        {
            DataContext = model;
            Closing += MainWindow_Closing;
            Messenger.Default.Register<BlockMessage>(this, OnBlockMessage);
            Messenger.Default.Register<NotifyMessage>(this, OnNotifyMessage);
            Messenger.Default.Register<ToastMessage>(this, OnToastMessage);
            Messenger.Default.Register<ConfirmMessage>(this, OnConfirmMessage);
            Messenger.Default.Register<ExitMessage>(this, OnExitMessage);
            InitializeComponent();
        }

        private async void BlockAsync(BlockMessage msg)
        {
            Log.Current.DebugFormat("Blocking: {0}", msg.Message);
            ProgressDialogController dialog = await this.ShowProgressAsync(
                msg.Title,
                msg.Message,
                false,
                new MetroDialogSettings
                {
                    AnimateHide = false
                });
            await msg.OnExecuting();
            await dialog.CloseAsync();
            msg.OnExecuted();
        }

        private async void NotifyAsync(NotifyMessage msg)
        {
            Log.Current.DebugFormat("Notifying: {0}", msg.Message);
            await this.ShowMessageAsync(
                msg.Title,
                msg.Message,
                MessageDialogStyle.Affirmative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "OK",
                    AnimateHide = false
                });
            msg.OnDismissed();
        }

        private async void ConfirmAsync(ConfirmMessage msg)
        {
            Log.Current.DebugFormat("Confirming: {0}", msg.Message);
            MessageDialogResult result = await this.ShowMessageAsync(
                msg.Title,
                msg.Message,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = msg.Verb,
                    NegativeButtonText = string.Format("Don't {0}", msg.Verb),
                    AnimateHide = false
                });
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Current.Debug("Confirmed");
                msg.OnConfirmed();
            }
            else
            {
                Log.Current.Debug("Canceled");
                msg.OnCanceled();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (App.Current.ShuttingDown || closing)
            {
                return;
            }
            e.Cancel = true;
            ConfirmMessage msg = new ConfirmMessage("Exit", string.Format("Are you sure you want to exit {0}?", App.Title));
            msg.Confirmed += (_sender, _e) =>
            {
                closing = true;
                Close();
                closing = false;
            };
            ConfirmAsync(msg);
        }

        private void OnBlockMessage(BlockMessage msg)
        {
            BlockAsync(msg);
        }

        private void OnNotifyMessage(NotifyMessage msg)
        {
            NotifyAsync(msg);
        }

        private void OnToastMessage(ToastMessage msg)
        {
            ToastPopUp popup = new ToastPopUp(App.Title, msg.Message, msg.NotificationType)
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black
            };
            popup.Show();
        }

        private void OnConfirmMessage(ConfirmMessage msg)
        {
            ConfirmAsync(msg);
        }

        private void OnExitMessage(ExitMessage msg)
        {
            Close();
        }
    }
}
