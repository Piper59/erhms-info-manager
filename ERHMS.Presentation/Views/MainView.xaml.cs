using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Controls;
using ERHMS.Presentation.Dialogs;
using ERHMS.Presentation.Services;
using ERHMS.Presentation.ViewModels;
using ERHMS.Utility;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Mantin.Controls.Wpf.Notification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using ResXResources = ERHMS.Presentation.Properties.Resources;

namespace ERHMS.Presentation.Views
{
    public partial class MainView : MetroWindow, IDialogService, IWrapperService
    {
        private IWin32Window owner;
        private ResourceDictionary accent;
        private bool closeRequested;
        private bool closing;

        public new MainViewModel DataContext
        {
            get { return (MainViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public MainView(MainViewModel model)
        {
            owner = new Win32Window(this);
            accent = (ResourceDictionary)Application.Current.Resources["Accent"];
            DataContext = model;
            InitializeComponent();
            Closing += MainView_Closing;
        }

        private async void MainView_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !closing;
            if (closeRequested || closing)
            {
                return;
            }
            closeRequested = true;
            if (await ConfirmAsync(ResXResources.AppConfirmExit, "Exit"))
            {
                closing = true;
                Close();
            }
            else
            {
                closeRequested = false;
            }
        }

        public IWin32Window GetOwner()
        {
            return owner;
        }

        public async Task AlertAsync(string message, string title = null)
        {
            Log.Logger.DebugFormat("Alerting: {0}", message);
            await this.ShowMessageAsync(
                title ?? "Error",
                message,
                settings: new MetroDialogSettings
                {
                    CustomResourceDictionary = accent,
                    AffirmativeButtonText = "OK"
                });
        }

        public async Task AlertAsync(ValidationError error, IEnumerable<string> fields)
        {
            string message = string.Format(ResXResources.ValidationError, error.ToString().ToLower(), string.Join(Environment.NewLine, fields));
            await AlertAsync(message);
        }

        public async Task BlockAsync(string message, Action action)
        {
            Log.Logger.DebugFormat("Blocking: {0}", message);
            ProgressDialogController controller = await this.ShowProgressAsync(
                "Working \u2026",
                message,
                settings: new MetroDialogSettings
                {
                    CustomResourceDictionary = accent
                });
            await Task.Run(action);
            await controller.CloseAsync();
        }

        public async Task<bool> ConfirmAsync(string message, string verb = null, string title = null)
        {
            Log.Logger.DebugFormat("Confirming: {0}", message);
            MessageDialogResult result = await this.ShowMessageAsync(
                title ?? string.Format("{0}?", verb),
                message,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    CustomResourceDictionary = accent,
                    AffirmativeButtonText = verb == null ? "OK" : verb,
                    NegativeButtonText = verb == null ? "Cancel" : string.Format("Don't {0}", verb)
                });
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Logger.Debug("Confirmed");
                return true;
            }
            else
            {
                Log.Logger.Debug("Canceled");
                return false;
            }
        }

        public void Notify(string message)
        {
            Log.Logger.DebugFormat("Notifying: {0}", message);
            ToastPopUp popup = new ToastPopUp(ResXResources.AppTitle, message, NotificationType.Information)
            {
                Background = Brushes.White,
                BorderBrush = Brushes.Black
            };
            popup.Show();
        }

        public async Task ShowAsync(DialogViewModel model)
        {
            Log.Logger.DebugFormat("Showing: {0}", model);
            model.Active = true;
            ChildWindow dialog = (ChildWindow)Activator.CreateInstance(DialogTypeMap.Instance.GetDialogType(model));
            dialog.DataContext = model;
            await this.ShowChildWindowAsync(dialog, ChildWindowManager.OverlayFillBehavior.FullWindow);
        }

        public async Task ShowErrorAsync(string message, Exception exception)
        {
            Log.Logger.DebugFormat("Alerting: {0}", message);
            await ErrorDialog.ShowAsync(this, message, exception);
        }

        public async Task<bool> ShowLicenseAsync()
        {
            Log.Logger.Debug("Showing license");
            bool result = await LicenseDialog.ShowAsync(this);
            Log.Logger.Debug(result ? "Accepted" : "Declined");
            return result;
        }

        public string OpenFolder()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                return dialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK ? dialog.SelectedPath : null;
            }
        }

        public string OpenFile(string title = null, string initialDirectory = null, string filter = null)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.InitialDirectory = initialDirectory;
                dialog.Filter = filter;
                return dialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK ? dialog.FileName : null;
            }
        }

        public IEnumerable<string> OpenFiles(string title = null, string initialDirectory = null, string filter = null)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = title;
                dialog.InitialDirectory = initialDirectory;
                dialog.Filter = filter;
                dialog.Multiselect = true;
                return dialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK ? dialog.FileNames : Enumerable.Empty<string>();
            }
        }

        public string SaveFile(string title = null, string initialDirectory = null, string filter = null, string fileName = null)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = title;
                dialog.InitialDirectory = initialDirectory;
                dialog.Filter = filter;
                dialog.FileName = fileName;
                return dialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK ? dialog.FileName : null;
            }
        }

        public string GetRootPath()
        {
            string directoryName = ResXResources.AppBareTitle;
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = ResXResources.RootPathInstructions;
                string path;
                while (true)
                {
                    Log.Logger.Debug("Prompting for root path");
                    path = null;
                    if (dialog.ShowDialog(owner) != System.Windows.Forms.DialogResult.OK)
                    {
                        Log.Logger.Debug("Canceled");
                        break;
                    }
                    path = Path.Combine(dialog.SelectedPath, directoryName);
                    Log.Logger.DebugFormat("Chosen: {0}", path);
                    if (!Directory.Exists(path))
                    {
                        break;
                    }
                    string message = string.Format(ResXResources.RootPathDirectoryExists, path);
                    if (MessageBox.Show(message, ResXResources.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        break;
                    }
                }
                return path;
            }
        }

        public async Task InvokeAsync(Wrapper wrapper)
        {
            WindowState state = WindowState;
            WindowState = WindowState.Minimized;
            wrapper.Invoke();
            await Task.Run(() =>
            {
                wrapper.Exited.WaitOne();
            });
            if (WindowState == WindowState.Minimized)
            {
                WindowState = state;
            }
        }
    }
}
