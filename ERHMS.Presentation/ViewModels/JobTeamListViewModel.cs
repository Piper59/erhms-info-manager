using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class JobTeamListViewModel : DocumentViewModel
    {
        public class TeamListChildViewModel : ListViewModel<Team>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Team);
                    yield return typeof(JobTeam);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public TeamListChildViewModel(Job job)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Team> GetItems()
            {
                return Context.Teams.SelectJobbable(Job.IncidentId, Job.JobId).OrderBy(team => team.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Team item)
            {
                yield return item.Name;
                yield return item.Description;
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Team.Equals(SelectedItem),
                    () => new TeamViewModel(Context.Teams.Refresh(SelectedItem)));
            }
        }

        public class JobTeamListChildViewModel : ListViewModel<JobTeam>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Team);
                    yield return typeof(JobTeam);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobTeamListChildViewModel(Job job)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<JobTeam> GetItems()
            {
                return Context.JobTeams.SelectByJobId(Job.JobId).OrderBy(jobTeam => jobTeam.Team.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(JobTeam item)
            {
                yield return item.Team.Name;
                yield return item.Team.Description;
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Team.Equals(SelectedItem.Team),
                    () => new TeamViewModel(Context.Teams.Refresh(SelectedItem.Team)));
            }
        }

        public Job Job { get; private set; }
        public TeamListChildViewModel Teams { get; private set; }
        public JobTeamListChildViewModel JobTeams { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public JobTeamListViewModel(Job job)
        {
            Title = "Teams";
            Job = job;
            Teams = new TeamListChildViewModel(job);
            JobTeams = new JobTeamListChildViewModel(job);
            AddCommand = new Command(Add, Teams.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobTeams.HasAnySelectedItems);
            EmailCommand = new Command(Email, JobTeams.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (ServiceLocator.Busy.Begin())
            {
                foreach (Team team in Teams.SelectedItems)
                {
                    Context.JobTeams.Save(new JobTeam(true)
                    {
                        JobId = Job.JobId,
                        TeamId = team.TeamId
                    });
                }
            }
            ServiceLocator.Data.Refresh(typeof(JobTeam));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.JobTeamConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (JobTeam jobTeam in JobTeams.SelectedItems)
                    {
                        Context.JobTeams.Delete(jobTeam);
                    }
                }
                ServiceLocator.Data.Refresh(typeof(JobTeam));
            }
        }

        public void Email()
        {
            ServiceLocator.Document.Show(() =>
            {
                ISet<Responder> responders = new HashSet<Responder>();
                foreach (JobTeam jobTeam in JobTeams.SelectedItems)
                {
                    responders.AddRange(Context.TeamResponders.SelectUndeletedByTeamId(jobTeam.TeamId)
                        .Select(teamResponder => teamResponder.Responder));
                }
                return new EmailViewModel(Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            Teams.Refresh();
            JobTeams.Refresh();
        }
    }
}
