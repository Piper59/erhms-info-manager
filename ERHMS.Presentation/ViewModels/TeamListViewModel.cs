using ERHMS.DataAccess;
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
    public class TeamListViewModel : DocumentViewModel
    {
        public class TeamListChildViewModel : ListViewModel<Team>
        {
            public Incident Incident { get; private set; }

            public TeamListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Team> GetItems()
            {
                return Context.Teams.SelectByIncidentId(Incident.IncidentId)
                    .WithResponders(Context)
                    .OrderBy(team => team.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Team item)
            {
                yield return item.Name;
                yield return item.Description;
                foreach (Responder responder in item.Responders)
                {
                    yield return responder.FullName;
                }
            }
        }

        public Incident Incident { get; private set; }
        public TeamListChildViewModel Teams { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public TeamListViewModel(Incident incident)
        {
            Title = "Teams";
            Incident = incident;
            Teams = new TeamListChildViewModel(incident);
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Teams.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Teams.HasSelectedItem);
        }

        public void Create()
        {
            ServiceLocator.Document.Show(() => new TeamViewModel(new Team(true)
            {
                IncidentId = Incident.IncidentId
            }));
        }

        public void Edit()
        {
            ServiceLocator.Document.Show(
                model => model.Team.Equals(Teams.SelectedItem),
                () => new TeamViewModel(Context.Teams.Refresh(Teams.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.TeamConfirmDelete, "Delete"))
            {
                Context.JobTeams.DeleteByTeamId(Teams.SelectedItem.TeamId);
                Context.TeamResponders.DeleteByTeamId(Teams.SelectedItem.TeamId);
                Context.Teams.Delete(Teams.SelectedItem);
                ServiceLocator.Data.Refresh(typeof(Team));
            }
        }
    }
}
