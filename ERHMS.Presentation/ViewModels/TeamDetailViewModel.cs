using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamDetailViewModel : ViewModelBase
    {
        public class TeamResponderListViewModel : ListViewModelBase<TeamResponderViewModel>
        {
            public Team Team { get; private set; }

            public TeamResponderListViewModel(Team team)
            {
                Team = team;
                Messenger.Default.Register<RefreshMessage<TeamResponder>>(this, msg => Refresh());
            }

            protected override IEnumerable<TeamResponderViewModel> GetItems()
            {
                ICollection<Responder> responders = DataContext.Responders.SelectUndeleted().ToList();
                ICollection<Role> roles = DataContext.Roles.Select().ToList();
                foreach (TeamResponder teamResponder in DataContext.TeamResponders.SelectByTeamId(Team.TeamId))
                {
                    Responder itemResponder = responders.SingleOrDefault(responder => responder.ResponderId.EqualsIgnoreCase(teamResponder.ResponderId));
                    if (itemResponder == null)
                    {
                        continue;
                    }
                    Role itemRole = roles.SingleOrDefault(role => role.RoleId.EqualsIgnoreCase(teamResponder.RoleId));
                    yield return new TeamResponderViewModel(teamResponder, itemResponder, itemRole);
                }
            }
        }

        public Team Team { get; private set; }
        public TeamResponderListViewModel TeamResponders { get; private set; }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public TeamDetailViewModel(Team team)
        {
            Team = team;
            AddDirtyCheck(team);
            Refresh();
            TeamResponders = new TeamResponderListViewModel(team);
            TeamResponders.Refresh();
            SaveCommand = new RelayCommand(Save);
            AddCommand = new RelayCommand(Add);
            RemoveCommand = new RelayCommand(Remove, TeamResponders.HasAnySelectedItems);
            EmailCommand = new RelayCommand(Email, TeamResponders.HasAnySelectedItems);
            RefreshCommand = new RelayCommand(TeamResponders.Refresh);
        }

        private void Refresh()
        {
            Title = Team.New ? "New Team" : Team.Name;
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Team.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                ShowRequiredMessage(fields);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            DataContext.Teams.Save(Team);
            Dirty = false;
            Messenger.Default.Send(new ToastMessage
            {
                Message = "Team has been saved."
            });
            Messenger.Default.Send(new RefreshMessage<Team>());
            Refresh();
        }

        public void Add()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new TeamResponderViewModel(Team)
                {
                    Active = true
                }
            });
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
                foreach (TeamResponderViewModel teamResponder in TeamResponders.SelectedItems)
                {
                    DataContext.TeamResponders.Delete(teamResponder.TeamResponder);
                }
                Messenger.Default.Send(new RefreshMessage<TeamResponder>());
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            Main.OpenEmailView(new EmailViewModel(TeamResponders.TypedSelectedItems.Select(teamResponder => teamResponder.Responder)));
        }
    }
}
