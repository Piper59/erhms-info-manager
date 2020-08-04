using ERHMS.Desktop.Properties;
using System;
using System.Threading;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        private static readonly TimeSpan ResizeTimerInterval = TimeSpan.FromSeconds(1.0);

        private Timer resizeTimer;

        public MainView()
        {
            InitializeComponent();
            Settings.Default.ApplyTo(this);
            resizeTimer = new Timer(state => Settings.Default.Save());
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Settings.Default.UpdateFrom(this);
            resizeTimer.Change(ResizeTimerInterval, Timeout.InfiniteTimeSpan);
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
