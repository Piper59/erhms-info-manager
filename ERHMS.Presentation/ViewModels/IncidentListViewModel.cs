using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<Incident, string>> FilterPropertyAccessors = new Func<Incident, string>[]
        {
            incident => incident.Name,
            incident => incident.Description,
            incident => EnumExtensions.ToDescription(incident.Phase),
            incident => incident.StartDate.HasValue ? incident.StartDate.Value.ToShortDateString() : null
        };

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (!Set(() => Filter, ref filter, value))
                {
                    return;
                }
                Incidents.Refresh();
            }
        }

        private ICollectionView incidents;
        public ICollectionView Incidents
        {
            get { return incidents; }
            set { Set(() => Incidents, ref incidents, value); }
        }

        private Incident selectedIncident;
        public Incident SelectedIncident
        {
            get
            {
                return selectedIncident;
            }
            set
            {
                if (!Set(() => SelectedIncident, ref selectedIncident, value))
                {
                    return;
                }
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentListViewModel()
        {
            Title = "Incidents";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(Open, HasSelectedIncident);
            DeleteCommand = new RelayCommand(Delete, HasSelectedIncident);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshMessage);
        }

        public bool HasSelectedIncident()
        {
            return SelectedIncident != null;
        }

        public void Create()
        {
            Locator.Main.OpenIncidentView(DataContext.Incidents.Create());
        }

        public void Open()
        {
            Locator.Main.OpenIncidentView((Incident)SelectedIncident.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this incident?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                SelectedIncident.Deleted = true;
                DataContext.Incidents.Save(SelectedIncident);
                Messenger.Default.Send(new RefreshMessage<Incident>());
            };
            Messenger.Default.Send(msg);
        }

        public void Refresh()
        {
            Incidents = CollectionViewSource.GetDefaultView(DataContext.Incidents
                .SelectByDeleted(false)
                .OrderBy(incident => incident.Name));
            Incidents.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Incident incident = (Incident)item;
            foreach (Func<Incident, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(incident);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<Incident> msg)
        {
            Refresh();
        }
    }
}
