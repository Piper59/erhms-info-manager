using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Views
{
    public partial class ProgressView : Window
    {
        private DispatcherTimer showTimer;

        public CancellationTokenSource Showing { get; }

        public ProgressView()
        {
            InitializeComponent();
            showTimer = new DispatcherTimer();
            showTimer.Tick += ShowTimer_Tick;
            Showing = new CancellationTokenSource();
            Showing.Token.Register(OnShowingCanceled);
        }

        public void ShowDialog(TimeSpan delay)
        {
            if (!Showing.IsCancellationRequested)
            {
                showTimer.Interval = delay;
                showTimer.Start();
            }
        }

        private void ShowTimer_Tick(object sender, EventArgs e)
        {
            showTimer.Stop();
            if (!Showing.IsCancellationRequested)
            {
                ShowDialog();
            }
        }

        private void OnShowingCanceled()
        {
            showTimer.Stop();
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!Showing.IsCancellationRequested)
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}
