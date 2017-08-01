using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentListViewModel : ListViewModel<Incident>
    {
        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create)); }
        }

        private RelayCommand openCommand;
        public ICommand OpenCommand
        {
            get { return openCommand ?? (openCommand = new RelayCommand(Open, HasSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSelectedItem)); }
        }

        public IncidentListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Incidents";
            SelectionChanged += (sender, e) =>
            {
                openCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
