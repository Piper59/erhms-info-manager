using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentListViewModel : ListViewModel<Incident>
    {
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        public IncidentListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Incidents";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Incident> GetItems()
        {
            return Context.Incidents.SelectUndeleted()
                .OrderByDescending(incident => incident.StartDate)
                .ThenBy(incident => incident.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Incident item)
        {
            yield return item.Name;
            yield return item.Description;
            yield return EnumExtensions.ToDescription(item.Phase);
            if (item.StartDate.HasValue)
            {
                yield return item.StartDate.Value.ToShortDateString();
            }
        }

        public void Create()
        {
            Documents.ShowNewIncident();
        }

        public void Open()
        {
            Documents.ShowIncident((Incident)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected incident?"
            };
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Deleted = true;
                Context.Incidents.Save(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Incident)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
