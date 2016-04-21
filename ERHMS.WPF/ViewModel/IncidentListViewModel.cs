using ERHMS.DataAccess;
using ERHMS.Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.WPF.ViewModel
{
    public class IncidentListViewModel : ViewModelBase
    {
        private Incident selectedIncident;
        public Incident SelectedIncident
        {
            get { return selectedIncident; }
            set { Set(() => SelectedIncident, ref selectedIncident, value); }
        }

        private CollectionViewSource incidentList;
        public ICollectionView IncidentList
        {
            get { return incidentList != null ? incidentList.View : null; }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set(() => Filter, ref filter, value);
                IncidentList.Filter = ListFilterFunc;
            }
        }

        private bool ListFilterFunc(object item)
        {
            Incident i = item as Incident;

            return Filter == null ||
                Filter.Equals("") ||
                (i.Name != null && i.Name.ToLower().Contains(Filter.ToLower())) ||
                (i.Phase.ToString().ToLower().Contains(Filter.ToLower())) ||
                (i.Description != null && i.Description.ToLower().Contains(Filter.ToLower()));
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentListViewModel()
        {
            RefreshIncidentData();

            AddCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowNewIncident"));
            });

            OpenCommand = new RelayCommand(() =>
            {
                Incident incident = (Incident)SelectedIncident.Clone();
                incident.New = false;
                Messenger.Default.Send(new NotificationMessage<Incident>(incident, "ShowEditIncident"));
            },
                HasSelectedIncident);
            DeleteCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<System.Action>(() =>
                {
                    App.GetDataContext().Incidents.Delete(SelectedIncident);
                }, "ConfirmDeleteIncident"));
            },
                HasSelectedIncident);
        }
        
        private bool HasSelectedIncident()
        {
            return SelectedIncident != null;
        }

        private void RefreshIncidentData()
        {
            incidentList = new CollectionViewSource();
            incidentList.Source = App.GetDataContext().Incidents.Select();
            IncidentList.Refresh();
            RaisePropertyChanged("IncidentList");
            SelectedIncident = null;
        }
    }
}
