using ERHMS.Presentation.Commands;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class DialogViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { SetProperty(nameof(Active), ref active, value); }
        }

        public ICommand CloseCommand { get; private set; }

        protected DialogViewModel()
        {
            CloseCommand = new Command(Close);
        }

        public void Close()
        {
            Active = false;
        }
    }
}
