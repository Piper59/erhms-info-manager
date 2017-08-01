using ERHMS.Domain;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

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
                    createCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create, HasName)); }
        }

        protected AnalysisViewModel(IServiceManager services, View view)
            : base(services)
        {
            View = view;
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public abstract void Create();
    }
}
