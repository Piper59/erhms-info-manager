using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobTeamListViewModel : ListViewModel<JobTeam>
    {
        public class TeamListChildViewModel : ListViewModel<Team>
        {
            public Job Job { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public TeamListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
                SelectionChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Team> GetItems()
            {
                return Context.Teams.SelectJobbable(Job.IncidentId, Job.JobId).OrderBy(team => team.Name);
            }

            protected override IEnumerable<string> GetFilteredValues(Team item)
            {
                yield return item.Name;
                yield return item.Description;
            }

            public void Edit()
            {
                Documents.ShowTeam((Team)SelectedItem.Clone());
            }
        }

        public Job Job { get; private set; }
        public TeamListChildViewModel Teams { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public JobTeamListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Teams";
            Job = job;
            Teams = new TeamListChildViewModel(services, job);
            Refresh();
            AddCommand = new RelayCommand(Add, Teams.HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            Teams.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<JobTeam> GetItems()
        {
            return Context.JobTeams.SelectByJobId(Job.JobId).OrderBy(jobTeam => jobTeam.Team.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(JobTeam item)
        {
            yield return item.Team.Name;
            yield return item.Team.Description;
        }

        public void Add()
        {
            foreach (Team team in Teams.SelectedItems)
            {
                Context.JobTeams.Save(new JobTeam(true)
                {
                    JobId = Job.JobId,
                    TeamId = team.TeamId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(JobTeam)));
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected teams?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (JobTeam jobTeam in SelectedItems)
                {
                    Context.JobTeams.Delete(jobTeam);
                }
                MessengerInstance.Send(new RefreshMessage(typeof(JobTeam)));
            };
            MessengerInstance.Send(msg);
        }

        public void Edit()
        {
            Documents.ShowTeam((Team)SelectedItem.Team.Clone());
        }

        public void Email()
        {
            ICollection<Responder> responders = new List<Responder>();
            foreach (JobTeam jobTeam in SelectedItems)
            {
                responders.AddRange(Context.Responders.SelectByTeamId(jobTeam.TeamId));
            }
            Documents.Show(
                () => new EmailViewModel(Services, responders.OrderBy(responder => responder.FullName)),
                document => false);
        }

        public override void Refresh()
        {
            Teams.Refresh();
            base.Refresh();
        }
    }
}
