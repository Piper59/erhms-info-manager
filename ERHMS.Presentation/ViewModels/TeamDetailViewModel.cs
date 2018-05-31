using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamDetailViewModel : DocumentViewModel
    {
        public Team Team { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public TeamDetailViewModel(Team team)
        {
            Title = team.New ? "New Team" : team.Name;
            Team = team;
            AddDirtyCheck(team);
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Team.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                await ServiceLocator.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            Context.Teams.Save(Team);
            ServiceLocator.Dialog.Notify(Resources.TeamSaved);
            ServiceLocator.Data.Refresh(typeof(Team));
            Title = Team.Name;
            Dirty = false;
        }
    }
}
