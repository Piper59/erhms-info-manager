using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class DocumentViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            protected set { Set(() => Title, ref title, value); }
        }

        private bool closed;
        public bool Closed
        {
            get { return closed; }
            private set { Set(() => Closed, ref closed, value); }
        }

        public RelayCommand CloseCommand { get; private set; }

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
