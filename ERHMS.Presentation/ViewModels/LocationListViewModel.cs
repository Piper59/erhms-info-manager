using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
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

            public LocationListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
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

        public LocationListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Locations";
            Incident = incident;
            Locations = new LocationListChildViewModel(services, incident);
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Locations.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Locations.HasSelectedItem);
        }

        public void Create()
        {
            Services.Document.Show(() => new LocationViewModel(Services, new Location(true)
            {
                IncidentId = Incident.IncidentId
            }));
        }

        public void Edit()
        {
            Services.Document.Show(
                model => model.Location.Equals(Locations.SelectedItem),
                () => new LocationViewModel(Services, Context.Locations.Refresh(Locations.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected location?", "Delete"))
            {
                Context.JobLocations.DeleteByLocationId(Locations.SelectedItem.LocationId);
                Context.Locations.Delete(Locations.SelectedItem);
                Services.Data.Refresh(typeof(Location));
            }
        }

        public override void Dispose()
        {
            Locations.Dispose();
            base.Dispose();
        }
    }
}
