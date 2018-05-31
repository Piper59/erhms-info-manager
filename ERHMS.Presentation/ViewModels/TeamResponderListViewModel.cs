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
    public class TeamResponderListViewModel : DocumentViewModel
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Team Team { get; private set; }

            public IncidentRoleListChildViewModel(Team team)
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
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(TeamResponder);
                }
            }

            public Team Team { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(Team team)
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
                return ListViewModelExtensions.GetFilteredValues(item);
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class TeamResponderListChildViewModel : ListViewModel<TeamResponder>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(TeamResponder);
                }
            }

            public Team Team { get; private set; }

            public ICommand EditCommand { get; private set; }

            public TeamResponderListChildViewModel(Team team)
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
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem.Responder)));
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

        public TeamResponderListViewModel(Team team)
        {
            Title = "Responders";
            Team = team;
            IncidentRoles = new IncidentRoleListChildViewModel(team);
            Responders = new ResponderListChildViewModel(team);
            TeamResponders = new TeamResponderListChildViewModel(team);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, TeamResponders.HasAnySelectedItems);
            EmailCommand = new Command(Email, TeamResponders.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (ServiceLocator.Busy.Begin())
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
            }
            ServiceLocator.Data.Refresh(typeof(TeamResponder));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.TeamResponderConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (TeamResponder teamResponder in TeamResponders.SelectedItems)
                    {
                        Context.TeamResponders.Delete(teamResponder);
                    }
                }
            }
            ServiceLocator.Data.Refresh(typeof(TeamResponder));
        }

        public void Email()
        {
            ServiceLocator.Document.Show(() =>
            {
                IEnumerable<Responder> responders = TeamResponders.SelectedItems.Select(teamResponder => teamResponder.Responder);
                return new EmailViewModel(Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            IncidentRoles.Refresh();
            Responders.Refresh();
            TeamResponders.Refresh();
        }
    }
}
