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
    public class LocationListViewModel : ViewModelBase
    {
        private static readonly ICollection<Func<Location, string>> FilterPropertyAccessors = new Func<Location, string>[]
        {
            location => location.Name,
            location => location.Description,
            location => location.Address,
            location => location.Latitude.ToString(),
            location => location.Longitude.ToString()
        };

        public Incident Incident { get; private set; }

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
                Locations.Refresh();
            }
        }

        private ICollectionView locations;
        public ICollectionView Locations
        {
            get { return locations; }
            set { Set(() => Locations, ref locations, value); }
        }

        private Location selectedLocation;
        public Location SelectedLocation
        {
            get
            {
                return selectedLocation;
            }
            set
            {
                if (!Set(() => SelectedLocation, ref selectedLocation, value))
                {
                    return;
                }
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public LocationListViewModel(Incident incident)
        {
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedLocation);
            DeleteCommand = new RelayCommand(Delete, HasSelectedLocation);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Location>>(this, OnRefreshMessage);
        }

        public bool HasSelectedLocation()
        {
            return SelectedLocation != null;
        }

        public void Create()
        {
            Location location = DataContext.Locations.Create();
            location.IncidentId = Incident.IncidentId;
            Locator.Main.OpenLocationDetailView(location);
        }

        public void Edit()
        {
            Locator.Main.OpenLocationDetailView((Location)SelectedLocation.Clone());
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
                DataContext.Locations.Delete(SelectedLocation);
                Messenger.Default.Send(new RefreshMessage<Location>());
            };
            Messenger.Default.Send(msg);
        }

        public void Refresh()
        {
            Locations = CollectionViewSource.GetDefaultView(DataContext.Locations
                .SelectByIncident(Incident.IncidentId)
                .OrderBy(location => location.Name));
            Locations.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Location location = (Location)item;
            foreach (Func<Location, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(location);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<Location> msg)
        {
            Refresh();
        }
    }
}
