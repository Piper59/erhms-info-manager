using ERHMS.Desktop.Properties;
using System;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            ReadSettings(Settings.Default);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            WriteSettings(Settings.Default);
            Settings.Default.DeferSave(TimeSpan.FromSeconds(1.0));
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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Settings.Default.FlushSave();
        }
    }
}
