using ERHMS.Desktop.Properties;
using System;
using System.Threading;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        private static readonly TimeSpan SaveSizeDelay = TimeSpan.FromSeconds(1.0);

        private Timer saveSizeTimer = new Timer(state => Settings.Default.Save());

        public MainView()
        {
            InitializeComponent();
            Settings.Default.ApplyTo(this);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Settings.Default.UpdateFrom(this);
            saveSizeTimer.Change(SaveSizeDelay, Timeout.InfiniteTimeSpan);
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
