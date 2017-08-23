using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobResponderListViewModel : ListViewModel<JobResponder>
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Job Job { get; private set; }

            public IncidentRoleListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
            }

            protected override IEnumerable<IncidentRole> GetItems()
            {
                return Context.IncidentRoles.SelectByIncidentId(Job.IncidentId)
                    .OrderBy(incidentRole => incidentRole.Name);
            }
        }

        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Job Job { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
                SelectionChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectJobbable(Job.IncidentId, Job.JobId).OrderBy(responder => responder.FullName);
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

        public Job Job { get; private set; }
        public IncidentRoleListChildViewModel IncidentRoles { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public JobResponderListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Responders";
            Job = job;
            IncidentRoles = new IncidentRoleListChildViewModel(services, job);
            Responders = new ResponderListChildViewModel(services, job);
            Refresh();
            AddCommand = new RelayCommand(Add, Responders.HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            Responders.SelectionChanged += (sender, e) =>
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

        protected override IEnumerable<JobResponder> GetItems()
        {
            return Context.JobResponders.SelectUndeletedByJobId(Job.JobId).OrderBy(jobResponder => jobResponder.Responder.FullName);
        }

        protected override IEnumerable<string> GetFilteredValues(JobResponder item)
        {
            yield return item.Responder.FullName;
            yield return item.IncidentRole?.Name;
        }

        public void Add()
        {
            foreach (Responder responder in Responders.SelectedItems)
            {
                Context.JobResponders.Save(new JobResponder(true)
                {
                    JobId = Job.JobId,
                    ResponderId = responder.ResponderId,
                    IncidentRoleId = IncidentRoles.SelectedItem?.IncidentRoleId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(JobResponder)));
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
                foreach (JobResponder jobResponder in SelectedItems)
                {
                    Context.JobResponders.Delete(jobResponder);
                }
                MessengerInstance.Send(new RefreshMessage(typeof(JobResponder)));
            };
            MessengerInstance.Send(msg);
        }

        public void Edit()
        {
            Documents.ShowResponder((Responder)SelectedItem.Responder.Clone());
        }

        public void Email()
        {
            Documents.Show(
                () => new EmailViewModel(Services, TypedSelectedItems.Select(jobResponder => jobResponder.Responder)),
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
