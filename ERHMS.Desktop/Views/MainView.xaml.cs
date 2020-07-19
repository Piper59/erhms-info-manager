using ERHMS.Desktop.Properties;
using System;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            Settings.Default.WriteTo(this);
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            Settings.Default.ReadFrom(this);
            Settings.Default.Save();
            base.OnClosed(e);
        }
    }
}
