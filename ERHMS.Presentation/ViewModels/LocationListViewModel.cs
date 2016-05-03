using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationListViewModel : ListViewModelBase<Location>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public LocationListViewModel(Incident incident)
        {
            Title = string.Format("{0} Locations", incident.Name);
            Incident = incident;
            Selecting += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Location>>(this, OnRefreshMessage);
        }

        protected override ICollectionView GetItems()
        {
            return CollectionViewSource.GetDefaultView(DataContext.Locations
                .SelectByIncident(Incident.IncidentId)
                .OrderBy(location => location.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Location item)
        {
            yield return item.Name;
            yield return item.Description;
            yield return item.Address;
            yield return item.Latitude.ToString();
            yield return item.Longitude.ToString();
        }

        public void Create()
        {
            Location location = DataContext.Locations.Create();
            location.IncidentId = Incident.IncidentId;
            Locator.Main.OpenLocationDetailView(location);
        }

        public void Edit()
        {
            Locator.Main.OpenLocationDetailView((Location)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this location?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.Locations.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Location>(Incident.IncidentId));
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshMessage(RefreshMessage<Location> msg)
        {
            if (msg.IncidentId == Incident.IncidentId)
            {
                Refresh();
            }
        }
    }
}
