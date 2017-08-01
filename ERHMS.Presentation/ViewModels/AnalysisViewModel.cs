using ERHMS.Domain;
using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class AnalysisViewModel : DialogViewModel
    {
        public View View { get; private set; }

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

        protected AnalysisViewModel(IServiceManager services, View view)
            : base(services)
        {
            View = view;
            CreateCommand = new RelayCommand(Create, HasName);
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public abstract void Create();
    }
}
