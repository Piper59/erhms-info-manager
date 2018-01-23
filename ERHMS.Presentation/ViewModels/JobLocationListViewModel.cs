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
    public class JobLocationListViewModel : DocumentViewModel
    {
        public class LocationListChildViewModel : ListViewModel<Location>
        {
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public LocationListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<Location> GetItems()
            {
                return Context.Locations.SelectJobbable(Job.IncidentId, Job.JobId)
                    .OrderBy(location => location.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(Location item)
            {
                yield return item.Name;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Location.Equals(SelectedItem),
                    () => new LocationViewModel(Services, Context.Locations.Refresh(SelectedItem)));
            }
        }

        public class JobLocationListChildViewModel : ListViewModel<JobLocation>
        {
            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobLocationListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                Refresh();
                EditCommand = new Command(Edit, HasSelectedItem);
            }

            protected override IEnumerable<JobLocation> GetItems()
            {
                return Context.JobLocations.SelectByJobId(Job.JobId)
                    .OrderBy(jobLocation => jobLocation.Location.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(JobLocation item)
            {
                yield return item.Location.Name;
            }

            public void Edit()
            {
                Services.Document.Show(
                    model => model.Location.Equals(SelectedItem.Location),
                    () => new LocationViewModel(Services, Context.Locations.Refresh(SelectedItem.Location)));
            }
        }

        public Job Job { get; private set; }
        public LocationListChildViewModel Locations { get; private set; }
        public JobLocationListChildViewModel JobLocations { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public JobLocationListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Locations";
            Job = job;
            Locations = new LocationListChildViewModel(services, job);
            JobLocations = new JobLocationListChildViewModel(services, job);
            AddCommand = new Command(Add, Locations.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobLocations.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            foreach (Location location in Locations.SelectedItems)
            {
                Context.JobLocations.Save(new JobLocation(true)
                {
                    JobId = Job.JobId,
                    LocationId = location.LocationId
                });
            }
            Locations.Refresh();
            Services.Data.Refresh(typeof(JobLocation));
        }

        public async Task RemoveAsync()
        {
            if (await Services.Dialog.ConfirmAsync("Remove the selected locations?", "Remove"))
            {
                foreach (JobLocation jobLocation in JobLocations.SelectedItems)
                {
                    Context.JobLocations.Delete(jobLocation);
                }
                Locations.Refresh();
                Services.Data.Refresh(typeof(JobLocation));
            }
        }

        public void Refresh()
        {
            Locations.Refresh();
            JobLocations.Refresh();
        }

        public override void Dispose()
        {
            Locations.Dispose();
            JobLocations.Dispose();
            base.Dispose();
        }
    }
}
