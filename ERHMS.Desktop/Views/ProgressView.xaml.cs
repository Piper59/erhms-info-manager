using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace ERHMS.Desktop.Views
{
    public partial class ProgressView : Window
    {
        private static readonly TimeSpan ShowDelay = TimeSpan.FromSeconds(1.0);

        private DispatcherTimer showTimer;
        private bool completed;

        public ProgressView()
        {
            showTimer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = ShowDelay
            };
            showTimer.Tick += ShowTimer_Tick;
            InitializeComponent();
        }

        public void BeginShowDialog()
        {
            completed = false;
            showTimer.Start();
        }

        private void ShowTimer_Tick(object sender, EventArgs e)
        {
            showTimer.Stop();
            if (!completed)
            {
                ShowDialog();
            }
        }

        public void EndShowDialog()
        {
            showTimer.Stop();
            completed = true;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel &= !completed;
            base.OnClosing(e);
        }
    }
}
