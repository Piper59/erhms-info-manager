using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
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

            public IncidentRoleListChildViewModel(Job job)
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
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(JobResponder);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public ResponderListChildViewModel(Job job)
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
                return ListViewModelExtensions.GetFilteredValues(item);
            }

            public void Edit()
            {
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem)));
            }
        }

        public class JobResponderListChildViewModel : ListViewModel<JobResponder>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Responder);
                    yield return typeof(JobResponder);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobResponderListChildViewModel(Job job)
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
                ServiceLocator.Document.Show(
                    model => model.Responder.Equals(SelectedItem.Responder),
                    () => new ResponderViewModel(Context.Responders.Refresh(SelectedItem.Responder)));
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

        public JobResponderListViewModel(Job job)
        {
            Title = "Responders";
            Job = job;
            IncidentRoles = new IncidentRoleListChildViewModel(job);
            Responders = new ResponderListChildViewModel(job);
            JobResponders = new JobResponderListChildViewModel(job);
            AddCommand = new Command(Add, Responders.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobResponders.HasAnySelectedItems);
            EmailCommand = new Command(Email, JobResponders.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (ServiceLocator.Busy.Begin())
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
            ServiceLocator.Data.Refresh(typeof(JobResponder));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.JobResponderConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (JobResponder jobResponder in JobResponders.SelectedItems)
                    {
                        Context.JobResponders.Delete(jobResponder);
                    }
                }
                ServiceLocator.Data.Refresh(typeof(JobResponder));
            }
        }

        public void Email()
        {
            ServiceLocator.Document.Show(() =>
            {
                IEnumerable<Responder> responders = JobResponders.SelectedItems.Select(jobResponder => jobResponder.Responder);
                return new EmailViewModel(Context.Responders.Refresh(responders));
            });
        }

        public void Refresh()
        {
            IncidentRoles.Refresh();
            Responders.Refresh();
            JobResponders.Refresh();
        }
    }
}
