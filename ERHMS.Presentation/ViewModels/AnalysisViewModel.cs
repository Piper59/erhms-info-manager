using GalaSoft.MvvmLight.Command;
using System;

namespace ERHMS.Presentation.ViewModels
{
    public class AnalysisViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(() => Active, ref active, value); }
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
                if (Set(() => Name, ref name, value))
                {
                    CreateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Action Create { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public AnalysisViewModel(Action create)
        {
            Create = create;
            CreateCommand = new RelayCommand(create, HasName);
            CancelCommand = new RelayCommand(Cancel);
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public void Reset()
        {
            Name = null;
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
