using ERHMS.Domain;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class LinkViewModelBase : ViewModelBase
    {
        public class IncidentListViewModel : ListViewModelBase<Incident>
        {
            public IncidentListViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<Incident> GetItems()
            {
                return DataContext.Incidents.SelectUndeleted().OrderBy(incident => incident.Name);
            }
        }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        public IncidentListViewModel Incidents { get; private set; }

        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand UnlinkCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        protected LinkViewModelBase(string incidentId)
        {
            Incidents = new IncidentListViewModel();
            Incidents.SelectItem(incident => incident.IncidentId.EqualsIgnoreCase(incidentId));
            LinkCommand = new RelayCommand(Link, Incidents.HasOneSelectedItem);
            UnlinkCommand = new RelayCommand(Unlink);
            CancelCommand = new RelayCommand(Cancel);
            Incidents.SelectedItemChanged += (sender, e) =>
            {
                LinkCommand.RaiseCanExecuteChanged();
            };
        }

        public abstract void Link();
        public abstract void Unlink();

        public void Cancel()
        {
            Active = false;
        }
    }
}
