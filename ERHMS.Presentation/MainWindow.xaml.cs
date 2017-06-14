using ERHMS.Presentation.Messages;
using ERHMS.Presentation.ViewModels;
using ERHMS.Presentation.Views;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Mantin.Controls.Wpf.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    public partial class MainWindow : MetroWindow
    {
        private static readonly IDictionary<Type, Type> DialogTypes = new Dictionary<Type, Type>
        {
            { typeof(DataSourceViewModel), typeof(DataSourceView) },
            { typeof(CanvasLinkViewModel), typeof(LinkView) },
            { typeof(CanvasViewModel), typeof(AnalysisView) },
            { typeof(PgmLinkViewModel), typeof(LinkView) },
            { typeof(PgmViewModel), typeof(AnalysisView) },
            { typeof(PrepopulateViewModel), typeof(PrepopulateView) },
            { typeof(RecipientViewModel), typeof(RecipientView) },
            { typeof(SurveyViewModel), typeof(SurveyView) },
            { typeof(ViewLinkViewModel), typeof(LinkView) }
        };

        private bool closing;

        public MainWindow()
        {
            DataContext = MainViewModel.Instance;
            Closing += MainWindow_Closing;
            Messenger.Default.Register<AlertMessage>(this, msg => AlertAsync(msg));
            Messenger.Default.Register<BlockMessage>(this, msg => BlockAsync(msg));
            Messenger.Default.Register<ConfirmMessage>(this, msg => ConfirmAsync(msg));
            Messenger.Default.Register<ToastMessage>(this, msg => Toast(msg));
            Messenger.Default.Register<ShowMessage>(this, msg => ShowAsync(msg));
            Messenger.Default.Register<ExitMessage>(this, msg => Close());
            InitializeComponent();
        }

        private async void AlertAsync(AlertMessage msg)
        {
            Log.Logger.DebugFormat("Alerting: {0}", msg.Message);
            await this.ShowMessageAsync(
                msg.Title,
                msg.Message,
                settings: new MetroDialogSettings
                {
                    CustomResourceDictionary = App.Current.Accent,
                    AffirmativeButtonText = "OK",
                    AnimateHide = false
                });
            msg.OnDismissed();
        }

        private async void BlockAsync(BlockMessage msg)
        {
            Log.Logger.DebugFormat("Blocking: {0}", msg.Message);
            ProgressDialogController dialog = await this.ShowProgressAsync(
                msg.Title,
                msg.Message,
                settings: new MetroDialogSettings
                {
                    CustomResourceDictionary = App.Current.Accent,
                    AnimateHide = false
                });
            await msg.OnExecuting();
            await dialog.CloseAsync();
            msg.OnExecuted();
        }

        private async void ConfirmAsync(ConfirmMessage msg)
        {
            Log.Logger.DebugFormat("Confirming: {0}", msg.Message);
            MessageDialogResult result = await this.ShowMessageAsync(
                msg.Title,
                msg.Message,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    CustomResourceDictionary = App.Current.Accent,
                    AffirmativeButtonText = msg.Verb,
                    NegativeButtonText = string.Format("Don't {0}", msg.Verb),
                    AnimateHide = false
                });
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Logger.Debug("Confirmed");
                msg.OnConfirmed();
            }
            else
            {
                Log.Logger.Debug("Canceled");
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
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Exit",
                Message = string.Format("Are you sure you want to exit {0}?", App.Title)
            };
            msg.Confirmed += (_sender, _e) =>
            {
                foreach (ViewModelBase document in MainViewModel.Instance.Documents.ToList())
                {
                    MainViewModel.Instance.Documents.Remove(document);
                }
                closing = true;
                Close();
                closing = false;
            };
            Messenger.Default.Send(msg);
        }

        private async void ShowAsync(ShowMessage msg)
        {
            ChildWindow dialog = (ChildWindow)Activator.CreateInstance(DialogTypes[msg.ViewModel.GetType()]);
            dialog.DataContext = msg.ViewModel;
            await this.ShowChildWindowAsync(dialog, ChildWindowManager.OverlayFillBehavior.FullWindow);
        }

        private void Toast(ToastMessage msg)
        {
            ToastPopUp popup = new ToastPopUp(App.Title, msg.Message, msg.Type)
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black
            };
            popup.Show();
        }
    }
}
