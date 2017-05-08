using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamListViewModel : ListViewModelBase<Team>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public TeamListViewModel(Incident incident)
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
            Messenger.Default.Register<RefreshMessage<Team>>(this, msg => Refresh());
        }

        protected override IEnumerable<Team> GetItems()
        {
            return DataContext.Teams.SelectByIncidentId(Incident.IncidentId).OrderBy(team => team.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Team item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Create()
        {
            Team team = DataContext.Teams.Create();
            team.IncidentId = Incident.IncidentId;
            Main.OpenTeamDetailView(team);
        }

        public void Edit()
        {
            Main.OpenTeamDetailView((Team)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected team?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.Teams.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Team>());
            };
            Messenger.Default.Send(msg);
        }
    }
}
