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
            Settings.Default.WriteTo(this);
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Settings.Default.ReadFrom(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.Save();
            base.OnClosed(e);
        }
    }
}
