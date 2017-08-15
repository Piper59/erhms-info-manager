using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Controls;
using ERHMS.Presentation.Dialogs;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace ERHMS.Presentation
{
    public partial class MainWindow : MetroWindow, IDialogManager
    {
        private static readonly IDictionary<Type, Type> DialogTypes = new Dictionary<Type, Type>
        {
            { typeof(CanvasLinkViewModel), typeof(LinkView) },
            { typeof(CanvasViewModel), typeof(AnalysisView) },
            { typeof(DataSourceViewModel), typeof(DataSourceView) },
            { typeof(PgmLinkViewModel), typeof(LinkView) },
            { typeof(PgmViewModel), typeof(AnalysisView) },
            { typeof(PrepopulateViewModel), typeof(PrepopulateView) },
            { typeof(RecipientViewModel), typeof(RecipientView) },
            { typeof(RoleViewModel), typeof(RoleView) },
            { typeof(SurveyViewModel), typeof(SurveyView) },
            { typeof(ViewLinkViewModel), typeof(LinkView) }
        };

        private bool closing;

        public IWin32Window Win32Window
        {
            get { return new Win32Window(this); }
        }

        public new MainViewModel DataContext
        {
            get { return (MainViewModel)base.DataContext; }
        }

        public MainWindow(MainViewModel dataContext)
        {
            InitializeComponent();
            base.DataContext = dataContext;
            Closing += MainWindow_Closing;
            Messenger.Default.Register<AlertMessage>(this, msg => AlertAsync(msg));
            Messenger.Default.Register<BlockMessage>(this, msg => BlockAsync(msg));
            Messenger.Default.Register<ConfirmMessage>(this, msg => ConfirmAsync(msg));
            Messenger.Default.Register<ExitMessage>(this, msg => Close());
            Messenger.Default.Register<ToastMessage>(this, msg => Toast(msg));
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
                foreach (ViewModelBase document in DataContext.Documents.ToList())
                {
                    DataContext.Documents.Remove(document);
                }
                closing = true;
                Close();
                closing = false;
            };
            Messenger.Default.Send(msg);
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

        public async Task InvokeAsync(Wrapper wrapper)
        {
            WindowState state = WindowState;
            WindowState = WindowState.Minimized;
            wrapper.Invoke();
            await Task.Factory.StartNew(() =>
            {
                wrapper.Exited.WaitOne();
            });
            if (WindowState == WindowState.Minimized)
            {
                WindowState = state;
            }
        }

        public async Task ShowAsync(DialogViewModel dataContext)
        {
            dataContext.Active = true;
            ChildWindow dialog = (ChildWindow)Activator.CreateInstance(DialogTypes[dataContext.GetType()]);
            dialog.DataContext = dataContext;
            await this.ShowChildWindowAsync(dialog, ChildWindowManager.OverlayFillBehavior.FullWindow);
        }

        public async Task ShowErrorAsync(string message, Exception exception)
        {
            await ErrorDialog.ShowAsync(this, message, exception);
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
