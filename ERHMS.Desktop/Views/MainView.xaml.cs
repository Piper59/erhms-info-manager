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
            Settings.Default.ApplyTo(this);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Settings.Default.UpdateFrom(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Settings.Default.Save();
        }

        private void DataContext_ExitRequested(object sender, EventArgs e)
        {
            Close();
        }
    }
}
