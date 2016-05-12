using ERHMS.DataAccess;
using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public ViewModelLocator Locator
        {
            get { return App.Current.Locator; }
        }

        protected DataContext DataContext
        {
            get { return Locator.Main.DataSource; }
        }

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

        public ViewModelBase()
        {
            CloseCommand = new RelayCommand(Close);
        }

        public void Close()
        {
            Closed = true;
        }
    }
}
