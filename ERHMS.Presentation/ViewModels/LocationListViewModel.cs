using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationListViewModel : ListViewModel<Location>
    {
        public Incident Incident { get; private set; }

        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create)); }
        }

        private RelayCommand editCommand;
        public ICommand EditCommand
        {
            get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        public LocationListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Locations";
            Incident = incident;
            SelectionChanged += (sender, e) =>
            {
                editCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
            Documents.ShowLocation(new Location
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
                Context.Locations.Delete(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Location)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
