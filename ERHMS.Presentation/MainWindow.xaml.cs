using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Mantin.Controls.Wpf.Notification;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    public partial class MainWindow : MetroWindow
    {
        private bool closing;

        public MainWindow(MainViewModel model)
        {
            DataContext = model;
            Closing += MainWindow_Closing;
            Messenger.Default.Register<BlockMessage>(this, OnBlockMessage);
            Messenger.Default.Register<ConfirmMessage>(this, OnConfirmMessage);
            Messenger.Default.Register<ToastMessage>(this, OnToastMessage);
            Messenger.Default.Register<ExitMessage>(this, OnExitMessage);
            InitializeComponent();
        }

        private Task RunTask(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
        }

        private async void BlockAsync(BlockMessage msg)
        {
            ProgressDialogController dialog = await this.ShowProgressAsync(
                msg.Title,
                msg.Message,
                false,
                new MetroDialogSettings
                {
                    AnimateHide = false
                });
            await RunTask(msg.OnExecuting);
            await dialog.CloseAsync();
        }

        private async void ConfirmAsync(ConfirmMessage msg)
        {
            MessageDialogResult result = await this.ShowMessageAsync(
                msg.Title,
                msg.Message,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = msg.AffirmativeButtonText,
                    NegativeButtonText = msg.NegativeButtonText,
                    AnimateHide = false
                });
            if (result == MessageDialogResult.Affirmative)
            {
                if (msg.Async)
                {
                    await RunTask(msg.OnConfirmed);
                }
                else
                {
                    msg.OnConfirmed();
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (closing)
            {
                return;
            }
            e.Cancel = true;
            ConfirmMessage msg = new ConfirmMessage(
                "Exit?",
                string.Format("Are you sure you want to exit {0}?", App.Title),
                "Exit",
                "Don't Exit");
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

        private void OnConfirmMessage(ConfirmMessage msg)
        {
            ConfirmAsync(msg);
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

        private void OnExitMessage(ExitMessage msg)
        {
            Close();
        }
    }
}
