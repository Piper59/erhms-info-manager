using ERHMS.DataAccess;
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
    public class TeamListViewModel : DocumentViewModel
    {
        public class TeamListChildViewModel : ListViewModel<Team>
        {
            public Incident Incident { get; private set; }

            public TeamListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
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

        public TeamListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Teams";
            Incident = incident;
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Teams.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Teams.HasSelectedItem);
        }

        public void Create()
        {
            Services.Document.Show(() => new TeamViewModel(Services, new Team(true)
            {
                IncidentId = Incident.IncidentId
            }));
        }

        public void Edit()
        {
            Services.Document.Show(
                model => model.Team.Equals(Teams.SelectedItem),
                () => new TeamViewModel(Services, Context.Teams.Refresh(Teams.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected team?", "Delete"))
            {
                Context.JobTeams.DeleteByTeamId(Teams.SelectedItem.TeamId);
                Context.TeamResponders.DeleteByTeamId(Teams.SelectedItem.TeamId);
                Context.Teams.Delete(Teams.SelectedItem);
                Services.Data.Refresh(typeof(Team));
            }
        }

        public override void Dispose()
        {
            Teams.Dispose();
            base.Dispose();
        }
    }
}
