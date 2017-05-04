using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

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
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Location>>(this, msg => Refresh());
        }

        protected override IEnumerable<Location> GetItems()
        {
            return DataContext.Locations.SelectByIncidentId(Incident.IncidentId).OrderBy(location => location.Name);
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
            Main.OpenLocationDetailView(location);
        }

        public void Edit()
        {
            Main.OpenLocationDetailView((Location)SelectedItem.Clone());
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
                DataContext.Locations.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Location>());
            };
            Messenger.Default.Send(msg);
        }
    }
}
