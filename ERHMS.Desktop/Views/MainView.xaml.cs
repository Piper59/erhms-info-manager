using ERHMS.Desktop.Properties;
using System;
using System.Threading;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        private static readonly TimeSpan settingsDelay = TimeSpan.FromSeconds(1.0);

        private static void SaveSettings(object _)
        {
            Settings.Default.Save();
        }

        private readonly Timer settingsTimer;

        public MainView()
        {
            InitializeComponent();
            ReadSettings(Settings.Default);
            settingsTimer = new Timer(SaveSettings);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            WriteSettings(Settings.Default);
            settingsTimer.Change(settingsDelay, Timeout.InfiniteTimeSpan);
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
    }
}
