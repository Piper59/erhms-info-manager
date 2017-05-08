using ERHMS.Domain;
using ERHMS.Utility;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamResponderViewModel : ViewModelBase
    {
        public Team Team { get; private set; }
        public TeamResponder TeamResponder { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private ICollection<Responder> responders;
        public ICollection<Responder> Responders
        {
            get { return responders; }
            set { Set(nameof(Responders), ref responders, value); }
        }

        private ICollection<Role> roles;
        public ICollection<Role> Roles
        {
            get { return roles; }
            set { Set(nameof(Roles), ref roles, value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { Set(nameof(Responder), ref responder, value); }
        }

        private Role role;
        public Role Role
        {
            get { return role; }
            set { Set(nameof(Role), ref role, value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public TeamResponderViewModel(Team team)
        {
            Team = team;
            ICollection<string> responderIds = DataContext.TeamResponders.SelectByTeamId(team.TeamId)
                .Select(teamResponder => teamResponder.ResponderId)
                .ToList();
            Responders = DataContext.Responders.SelectByIncidentId(team.IncidentId)
                .Where(responder => !responderIds.ContainsIgnoreCase(responder.ResponderId))
                .OrderBy(responder => responder.FullName)
                .ToList();
            Roles = DataContext.Roles.Select()
                .OrderBy(role => role.Name)
                .ToList();
            AddCommand = new RelayCommand(Add, HasResponder);
            CancelCommand = new RelayCommand(Cancel);
        }

        public TeamResponderViewModel(TeamResponder teamResponder, Responder responder, Role role)
        {
            TeamResponder = teamResponder;
            Responder = responder;
            Role = role;
        }

        public bool HasResponder()
        {
            return Responder != null;
        }

        public void Add()
        {
            TeamResponder teamResponder = DataContext.TeamResponders.Create();
            teamResponder.TeamId = Team.TeamId;
            teamResponder.ResponderId = Responder.ResponderId;
            teamResponder.RoleId = Role?.RoleId;
            DataContext.TeamResponders.Save(teamResponder);
            Messenger.Default.Send(new RefreshMessage<TeamResponder>());
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
