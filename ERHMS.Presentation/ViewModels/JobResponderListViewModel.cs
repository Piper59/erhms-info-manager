using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class JobResponderListViewModel : DocumentViewModel
    {
        public class IncidentRoleListChildViewModel : ListViewModel<IncidentRole>
        {
            public Job Job { get; private set; }

            public IncidentRoleListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                Refresh();
            }

            protected override IEnumerable<IncidentRole> GetItems()
            {
                return Context.IncidentRoles.SelectByIncidentId(Job.IncidentId)
                    .OrderBy(incidentRole => incidentRole.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectJobbable(Job.IncidentId, Job.JobId)
                    .OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
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
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class JobResponderListChildViewModel : ListViewModel<JobResponder>
        {
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobResponderListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<JobResponder> GetItems()
            {
                return Context.JobResponders.SelectUndeletedByJobId(Job.JobId)
                    .OrderBy(jobResponder => jobResponder.Responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(JobResponder item)
            {
                yield return item.Responder.FullName;
                yield return item.IncidentRole?.Name;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Services, Context.Responders.Refresh(SelectedItem.Responder)));
            }
        }

        public Job Job { get; private set; }
        public IncidentRoleListChildViewModel IncidentRoles { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }
        public JobResponderListChildViewModel JobResponders { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand EmailCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public JobResponderListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Responders";
            Job = job;
            IncidentRoles = new IncidentRoleListChildViewModel(services, job);
            Responders = new ResponderListChildViewModel(services, job);
            JobResponders = new JobResponderListChildViewModel(services, job);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobResponders.HasAnySelectedItems);
            EmailCommand = new Command(Email, JobResponders.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (Services.Busy.BeginTask())
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
            }
            Responders.Refresh();
            Services.Data.Refresh(typeof(JobResponder));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected responders?", "Remove"))
            {
                using (Services.Busy.BeginTask())
                {
                    foreach (JobResponder jobResponder in JobResponders.SelectedItems)
                    {
                        Context.JobResponders.Delete(jobResponder);
                    }
                }
                Responders.Refresh();
                Services.Data.Refresh(typeof(JobResponder));
            }
        }

        public void Email()
        {
            Services.Document.Show(() =>
            {
                IEnumerable<Responder> responders = JobResponders.SelectedItems.Select(jobResponder => jobResponder.Responder);
                return new EmailViewModel(Services, Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            IncidentRoles.Refresh();
            Responders.Refresh();
            JobResponders.Refresh();
        }

        public override void Dispose()
        {
            IncidentRoles.Dispose();
            Responders.Dispose();
            JobResponders.Dispose();
            base.Dispose();
        }
    }
}
