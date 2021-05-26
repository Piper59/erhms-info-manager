using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ERHMS.Desktop.Views
{
    public partial class MainView : Window, IContentService
    {
        public new MainViewModel DataContext
        {
            get { return (MainViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public ICommand ExitCommand { get; }

        public MainView()
        {
            ExitCommand = new SyncCommand(Exit);
            InitializeComponent();
            ReadSettings(Settings.Default);
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

        public bool? ShowContent(object dataContext)
        {
            Log.Instance.Debug($"Showing content: {dataContext}");
            object key = new DataTemplateKey(dataContext.GetType());
            DataTemplate template = (DataTemplate)FindResource(key);
            FrameworkElement element = (FrameworkElement)template.LoadContent();
            element.DataContext = dataContext;
            if (element is Window window)
            {
                return window.ShowDialog();
            }
            else
            {
                ContentPresenter.Content = element;
                return null;
            }
        }

        bool? IContentService.Show(object dataContext)
        {
            return ShowContent(dataContext);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel)
            {
                return;
            }
            WriteSettings(Settings.Default);
            Settings.Default.Save();
        }

        public void Exit()
        {
            Close();
        }
    }
}
