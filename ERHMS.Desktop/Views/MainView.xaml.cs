using ERHMS.Desktop.Properties;
using System;
using System.Windows;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            try
            {
                Settings.Default.WriteTo(this);
            }
            catch { }
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                Settings.Default.ReadFrom(this);
                Settings.Default.Save();
            }
            catch { }
            base.OnClosed(e);
        }
    }
}
