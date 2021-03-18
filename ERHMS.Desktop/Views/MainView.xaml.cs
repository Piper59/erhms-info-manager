using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using System;
using System.ComponentModel;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window, INotificationService
    {
        public MainView()
        {
            InitializeComponent();
            ReadSettings(Settings.Default);
        }

        private void ReadSettings(Settings settings)
        {
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;
            WindowState = settings.WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void WriteSettings(Settings settings)
        {
            settings.WindowWidth = RestoreBounds.Width;
            settings.WindowHeight = RestoreBounds.Height;
            settings.WindowMaximized = WindowState == WindowState.Maximized;
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel)
            {
                return;
            }
            WriteSettings(Settings.Default);
            Settings.Default.Save();
        }

        public void Notify(string message)
        {
            Toaster.Show(message);
        }
    }
}
