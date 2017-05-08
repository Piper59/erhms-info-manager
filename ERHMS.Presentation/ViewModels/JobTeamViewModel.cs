using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobTeamViewModel : ViewModelBase
    {
        public Job Job { get; private set; }
        public JobTeam JobTeam { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private ICollection<Team> teams;
        public ICollection<Team> Teams
        {
            get { return teams; }
            set { Set(nameof(Teams), ref teams, value); }
        }

        private Team team;
        public Team Team
        {
            get { return team; }
            set { Set(nameof(Team), ref team, value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public JobTeamViewModel(Job job)
        {
            Job = job;
            ICollection<string> teamIds = DataContext.JobTeams.SelectByJobId(job.JobId)
                .Select(jobTeam => jobTeam.TeamId)
                .ToList();
            Teams = DataContext.Teams.SelectByIncidentId(job.IncidentId)
                .Where(team => !teamIds.ContainsIgnoreCase(team.TeamId))
                .OrderBy(team => team.Name)
                .ToList();
            AddCommand = new RelayCommand(Add, HasTeam);
            CancelCommand = new RelayCommand(Cancel);
        }

        public JobTeamViewModel(JobTeam jobTeam, Team team)
        {
            JobTeam = jobTeam;
            Team = team;
        }

        public bool HasTeam()
        {
            return Team != null;
        }

        public void Add()
        {
            JobTeam jobTeam = DataContext.JobTeams.Create();
            jobTeam.JobId = Job.JobId;
            jobTeam.TeamId = Team.TeamId;
            DataContext.JobTeams.Save(jobTeam);
            Messenger.Default.Send(new RefreshMessage<JobTeam>());
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
