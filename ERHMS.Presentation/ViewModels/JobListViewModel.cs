using ERHMS.DataAccess;
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
    public class JobListViewModel : DocumentViewModel
    {
        public class JobListChildViewModel : ListViewModel<Job>
        {
            public Incident Incident { get; private set; }

            public JobListChildViewModel(IServiceManager services, Incident incident)
                : base(services)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<Job> GetItems()
            {
                return Context.Jobs.SelectByIncidentId(Incident.IncidentId)
                    .WithResponders(Context)
                    .OrderByDescending(job => job.StartDate)
                    .ThenBy(job => job.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Job item)
            {
                yield return item.Name;
                yield return item.Description;
                if (item.StartDate.HasValue)
                {
                    yield return item.StartDate.Value.ToShortDateString();
                }
                foreach (Responder responder in item.Responders)
                {
                    yield return responder.FullName;
                }
            }
        }

        public Incident Incident { get; private set; }
        public JobListChildViewModel Jobs { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public JobListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Jobs";
            Incident = incident;
            Jobs = new JobListChildViewModel(services, incident);
            CreateCommand = new Command(Create);
            EditCommand = new Command(Edit, Jobs.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Jobs.HasSelectedItem);
        }

        public void Create()
        {
            Services.Document.Show(() => new JobViewModel(Services, new Job(true)
            {
                IncidentId = Incident.IncidentId
            }));
        }

        public void Edit()
        {
            Services.Document.Show(
                model => model.Job.Equals(Jobs.SelectedItem),
                () => new JobViewModel(Services, Context.Jobs.Refresh(Jobs.SelectedItem)));
        }

        public async Task DeleteAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Delete the selected job?", "Delete"))
            {
                Context.JobNotes.DeleteByJobId(Jobs.SelectedItem.JobId);
                Context.JobTeams.DeleteByJobId(Jobs.SelectedItem.JobId);
                Context.JobResponders.DeleteByJobId(Jobs.SelectedItem.JobId);
                Context.JobLocations.DeleteByJobId(Jobs.SelectedItem.JobId);
                Context.Jobs.Delete(Jobs.SelectedItem);
                Services.Data.Refresh(typeof(Job));
            }
        }

        public override void Dispose()
        {
            Jobs.Dispose();
            base.Dispose();
        }
    }
}
