using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window
    {
        public new MainViewModel DataContext
        {
            get { return (MainViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand CloseCommand { get; }

        public MainView()
        {
            CloseCommand = new SyncCommand(Close);
            InitializeComponent();
            ReadConfiguration(Configuration.Instance);
        }

        private void ReadConfiguration(Configuration configuration)
        {
            Width = configuration.WindowWidth;
            Height = configuration.WindowHeight;
            WindowState = configuration.WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void WriteConfiguration(Configuration configuration)
        {
            configuration.WindowWidth = RestoreBounds.Width;
            configuration.WindowHeight = RestoreBounds.Height;
            configuration.WindowMaximized = WindowState == WindowState.Maximized;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel)
            {
                return;
            }
            WriteConfiguration(Configuration.Instance);
            Configuration.Instance.Save();
        }
    }
}
