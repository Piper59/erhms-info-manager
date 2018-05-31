using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationListViewModel : DocumentViewModel
    {
        public class LocationListChildViewModel : ListViewModel<Location>
        {
            public Incident Incident { get; private set; }

            public LocationListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Location> GetItems()
            {
                return Context.Locations.SelectByIncidentId(Incident.IncidentId)
                    .OrderBy(location => location.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Location item)
            {
                yield return item.Name;
                yield return item.Description;
                yield return item.Address;
                yield return item.Latitude.ToString();
                yield return item.Longitude.ToString();
            }
        }

        public Incident Incident { get; private set; }
        public LocationListChildViewModel Locations { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public LocationListViewModel(Incident incident)
        {
            Title = "Locations";
            Incident = incident;
            Locations = new LocationListChildViewModel(incident);
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Locations.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Locations.HasSelectedItem);
        }

        public void Create()
        {
            ServiceLocator.Document.Show(() => new LocationViewModel(new Location(true)
            {
                IncidentId = Incident.IncidentId
            }));
        }

        public void Edit()
        {
            ServiceLocator.Document.Show(
                model => model.Location.Equals(Locations.SelectedItem),
                () => new LocationViewModel(Context.Locations.Refresh(Locations.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.LocationConfirmDelete, "Delete"))
            {
                Context.JobLocations.DeleteByLocationId(Locations.SelectedItem.LocationId);
                Context.Locations.Delete(Locations.SelectedItem);
                ServiceLocator.Data.Refresh(typeof(Location));
            }
        }
    }
}
