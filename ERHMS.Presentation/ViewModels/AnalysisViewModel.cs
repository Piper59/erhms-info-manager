using Epi;
using ERHMS.Domain;
using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class AnalysisViewModel : ViewModelBase
    {
        public DeepLink<View> ViewDeepLink { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (Set(nameof(Name), ref name, value))
                {
                    CreateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        protected AnalysisViewModel(DeepLink<View> viewDeepLink)
        {
            ViewDeepLink = viewDeepLink;
            CreateCommand = new RelayCommand(Create, HasName);
            CancelCommand = new RelayCommand(Cancel);
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public abstract void Create();

        public void Cancel()
        {
            Active = false;
        }
    }
}
