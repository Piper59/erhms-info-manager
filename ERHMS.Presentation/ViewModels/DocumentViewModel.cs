using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class DocumentViewModel : ViewModelBase
    {
        public abstract string Title { get; }

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            set { Set(() => Closed, ref closed, value); }
        }

        public ICommand CloseCommand { get; private set; }

        public DocumentViewModel()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public void Close()
        {
            Closed = true;
        }
    }
}
