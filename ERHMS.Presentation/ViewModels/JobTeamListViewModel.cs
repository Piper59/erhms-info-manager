using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
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
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public TeamListChildViewModel(IServiceManager services, Job job)
                : base(services)
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
                Services.Document.Show(
                    model => model.Team.Equals(SelectedItem),
                    () => new TeamViewModel(Services, Context.Teams.Refresh(SelectedItem)));
            }
        }

        public class JobTeamListChildViewModel : ListViewModel<JobTeam>
        {
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobTeamListChildViewModel(IServiceManager services, Job job)
                : base(services)
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
                Services.Document.Show(
                    model => model.Team.Equals(SelectedItem.Team),
                    () => new TeamViewModel(Services, Context.Teams.Refresh(SelectedItem.Team)));
            }
        }

        public Job Job { get; private set; }
        public TeamListChildViewModel Teams { get; private set; }
        public JobTeamListChildViewModel JobTeams { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public JobTeamListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Teams";
            Job = job;
            Teams = new TeamListChildViewModel(services, job);
            JobTeams = new JobTeamListChildViewModel(services, job);
            AddCommand = new Command(Add, Teams.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobTeams.HasAnySelectedItems);
            EmailCommand = new Command(Email, JobTeams.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (Services.Busy.BeginTask())
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
            Teams.Refresh();
            Services.Data.Refresh(typeof(JobTeam));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected teams?", "Remove"))
            {
                using (Services.Busy.BeginTask())
                {
                    foreach (JobTeam jobTeam in JobTeams.SelectedItems)
                    {
                        Context.JobTeams.Delete(jobTeam);
                    }
                }
                Teams.Refresh();
                Services.Data.Refresh(typeof(JobTeam));
            }
        }

        public void Email()
        {
            Services.Document.Show(() =>
            {
                ISet<Responder> responders = new HashSet<Responder>();
                foreach (JobTeam jobTeam in JobTeams.SelectedItems)
                {
                    responders.AddRange(Context.TeamResponders.SelectUndeletedByTeamId(jobTeam.TeamId)
                        .Select(teamResponder => teamResponder.Responder));
                }
                return new EmailViewModel(Services, Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            Teams.Refresh();
            JobTeams.Refresh();
        }

        public override void Dispose()
        {
            Teams.Dispose();
            JobTeams.Dispose();
            base.Dispose();
        }
    }
}
