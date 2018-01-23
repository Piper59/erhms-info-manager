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
    public class TeamResponderListViewModel : DocumentViewModel
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Team Team { get; private set; }

            public IncidentRoleListChildViewModel(IServiceManager services, Team team)
                : base(services)
            {
                Team = team;
                Refresh();
            }

            protected override IEnumerable<IncidentRole> GetItems()
            {
                return Context.IncidentRoles.SelectByIncidentId(Team.IncidentId)
                    .OrderBy(incidentRole => incidentRole.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Team Team { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Team team)
                : base(services)
            {
                Team = team;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectTeamable(Team.IncidentId, Team.TeamId)
                    .OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Responder item)
            {
                yield return item.LastName;
                yield return item.FirstName;
                yield return item.EmailAddress;
                yield return item.City;
                yield return item.State;
                yield return item.OrganizationName;
                yield return item.Occupation;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class TeamResponderListChildViewModel : ListViewModel<TeamResponder>
        {
            public Team Team { get; private set; }

            public ICommand EditCommand { get; private set; }

            public TeamResponderListChildViewModel(IServiceManager services, Team team)
                : base(services)
            {
                Team = team;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<TeamResponder> GetItems()
            {
                return Context.TeamResponders.SelectUndeletedByTeamId(Team.TeamId)
                    .OrderBy(teamResponder => teamResponder.Responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(TeamResponder item)
            {
                yield return item.Responder.FullName;
                yield return item.IncidentRole?.Name;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem.Responder)));
            }
        }

        public Team Team { get; private set; }
        public IncidentRoleListChildViewModel IncidentRoles { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public TeamResponderListChildViewModel TeamResponders { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public TeamResponderListViewModel(IServiceManager services, Team team)
            : base(services)
        {
            Title = "Responders";
            Team = team;
            IncidentRoles = new IncidentRoleListChildViewModel(services, team);
            Responders = new ResponderListChildViewModel(services, team);
            TeamResponders = new TeamResponderListChildViewModel(services, team);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, TeamResponders.HasAnySelectedItems);
            EmailCommand = new Command(Email, TeamResponders.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.TeamResponders.Save(new TeamResponder(true)
                {
                    TeamId = Team.TeamId,
                    ResponderId = responder.ResponderId,
                    IncidentRoleId = IncidentRoles.SelectedItem?.IncidentRoleId
                });
            }
            Responders.Refresh();
            Services.Data.Refresh(typeof(TeamResponder));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected responders?", "Remove"))
            {
                foreach (TeamResponder teamResponder in TeamResponders.SelectedItems)
                {
                    Context.TeamResponders.Delete(teamResponder);
                }
            }
            Responders.Refresh();
            Services.Data.Refresh(typeof(TeamResponder));
        }

        public void Email()
        {
            Services.Document.Show(() =>
            {
                IEnumerable<Responder> responders = TeamResponders.SelectedItems.Select(teamResponder => teamResponder.Responder);
                return new EmailViewModel(Services, Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            IncidentRoles.Refresh();
            Responders.Refresh();
            TeamResponders.Refresh();
        }

        public override void Dispose()
        {
            IncidentRoles.Dispose();
            Responders.Dispose();
            TeamResponders.Dispose();
            base.Dispose();
        }
    }
}
