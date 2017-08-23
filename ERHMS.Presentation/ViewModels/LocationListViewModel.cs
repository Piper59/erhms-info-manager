using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationListViewModel : ListViewModel<Location>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        public LocationListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Locations";
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Location> GetItems()
        {
            return Context.Locations.SelectByIncidentId(Incident.IncidentId).OrderBy(location => location.Name);
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
            Documents.ShowLocation(new Location(true)
            {
                IncidentId = Incident.IncidentId
            });
        }

        public void Edit()
        {
            Documents.ShowLocation((Location)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected location?"
            };
            msg.Confirmed += (sender, e) =>
            {
                Context.JobLocations.DeleteByLocationId(SelectedItem.LocationId);
                Context.Locations.Delete(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Location)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
