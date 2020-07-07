using ERHMS.Desktop.Commands;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private object content = new HomeViewModel();
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                App.Log.Debug($"Displaying {value}");
                Set(ref content, value);
            }
        }

        public ICommand HomeCommand { get; }

        public MainViewModel()
        {
            HomeCommand = new Command(Home);
        }

        private void Home()
        {
            Content = new HomeViewModel();
        }
    }
}
