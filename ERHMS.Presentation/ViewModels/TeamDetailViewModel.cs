using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamDetailViewModel : ViewModelBase
    {
        public Team Team { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public TeamDetailViewModel(IServiceManager services, Team team)
            : base(services)
        {
            Title = team.New ? "New Team" : team.Name;
            Team = team;
            SaveCommand = new RelayCommand(Save);
            AddDirtyCheck(team);
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
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Context.Teams.Save(Team);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Team has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Team)));
            Title = Team.Name;
            Dirty = false;
        }
    }
}
