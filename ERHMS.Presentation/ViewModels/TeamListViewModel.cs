using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamListViewModel : ListViewModel<Team>
    {
        public Incident Incident { get; private set; }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }

        public TeamListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Teams";
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

        protected override IEnumerable<Team> GetItems()
        {
            return Context.Teams.SelectByIncidentId(Incident.IncidentId).OrderBy(team => team.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Team item)
        {
            yield return item.Name;
            yield return item.Description;
        }

        public void Create()
        {
            Documents.ShowTeam(new Team(true)
            {
                IncidentId = Incident.IncidentId,
                Incident = Incident
            });
        }

        public void Edit()
        {
            Documents.ShowTeam((Team)SelectedItem.Clone());
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
                Context.JobTeams.DeleteByTeamId(SelectedItem.TeamId);
                Context.Teams.Delete(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Team)));
            };
            MessengerInstance.Send(msg);
        }
    }
}
