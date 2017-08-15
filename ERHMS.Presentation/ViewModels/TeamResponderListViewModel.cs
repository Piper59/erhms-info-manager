using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamResponderListViewModel : ListViewModel<TeamResponder>
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Incident Incident { get; private set; }

            public IncidentRoleListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
            }

            protected override IEnumerable<IncidentRole> GetItems()
            {
                return Context.IncidentRoles.SelectByIncidentId(Incident.IncidentId)
                    .OrderBy(incidentRole => incidentRole.Name);
            }
        }

        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Team Team { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Team team)
                : base(services)
            {
                Team = team;
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
                SelectionChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectTeamable(Team.IncidentId, Team.TeamId).OrderBy(responder => responder.FullName);
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
                Documents.ShowResponder((Responder)SelectedItem.Clone());
            }
        }

        public Team Team { get; private set; }
        public IncidentRoleListChildViewModel IncidentRoles { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public TeamResponderListViewModel(IServiceManager services, Team team)
            : base(services)
        {
            Title = "Responders";
            Team = team;
            IncidentRoles = new IncidentRoleListChildViewModel(services, team.Incident);
            Responders = new ResponderListChildViewModel(services, team);
            Refresh();
            AddCommand = new RelayCommand(Add, Responders.HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            Responders.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<TeamResponder> GetItems()
        {
            return Context.TeamResponders.SelectUndeletedByTeamId(Team.TeamId).OrderBy(teamResponder => teamResponder.Responder.FullName);
        }

        protected override IEnumerable<string> GetFilteredValues(TeamResponder item)
        {
            yield return item.Responder.FullName;
            yield return item.IncidentRole?.Name;
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.TeamResponders.Save(new TeamResponder
                {
                    TeamId = Team.TeamId,
                    ResponderId = responder.ResponderId,
                    IncidentRoleId = IncidentRoles.SelectedItem?.IncidentRoleId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(TeamResponder)));
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected responders?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (TeamResponder teamResponder in SelectedItems)
                {
                    Context.TeamResponders.Delete(teamResponder);
                }
                MessengerInstance.Send(new RefreshMessage(typeof(TeamResponder)));
            };
            MessengerInstance.Send(msg);
        }

        public void Email()
        {
            Documents.Show(
                () => new EmailViewModel(Services, TypedSelectedItems.Select(teamResponder => teamResponder.Responder)),
                document => false);
        }

        public override void Refresh()
        {
            IncidentRoles.Refresh();
            Responders.Refresh();
            base.Refresh();
        }
    }
}
