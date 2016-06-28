using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class LinkViewModelBase : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(() => Active, ref active, value); }
        }

        private ICollection<Incident> incidents;
        public ICollection<Incident> Incidents
        {
            get { return incidents; }
            set { Set(() => Incidents, ref incidents, value); }
        }

        private Incident selectedIncident;
        public Incident SelectedIncident
        {
            get { return selectedIncident; }
            set { Set(() => SelectedIncident, ref selectedIncident, value); }
        }

        public string SelectedIncidentId
        {
            get { return SelectedIncident == null ? null : SelectedIncident.IncidentId; }
        }

        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand UnlinkCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public LinkViewModelBase()
        {
            Refresh();
            LinkCommand = new RelayCommand(Link, HasSelectedIncident);
            UnlinkCommand = new RelayCommand(Unlink);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool HasSelectedIncident()
        {
            return SelectedIncident != null;
        }

        private void Refresh()
        {
            Incidents = DataContext.Incidents.SelectByDeleted(false)
                .OrderBy(incident => incident.Name)
                .ToList();
        }

        public void Reset(string selectedIncidentId)
        {
            SelectedIncident = Incidents.SingleOrDefault(incident => incident.IncidentId.EqualsIgnoreCase(selectedIncidentId));
        }

        public abstract void Link();
        public abstract void Unlink();

        public void Cancel()
        {
            Active = false;
        }

        private void OnRefreshIncidentListMessage(RefreshListMessage<Incident> msg)
        {
            Refresh();
        }
    }
}
