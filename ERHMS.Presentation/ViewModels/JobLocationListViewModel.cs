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
    public class JobLocationListViewModel : DocumentViewModel
    {
        public class LocationListChildViewModel : ListViewModel<Location>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Location);
                    yield return typeof(JobLocation);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public LocationListChildViewModel(Job job)
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
                ServiceLocator.Document.Show(
                    model => model.Location.Equals(SelectedItem),
                    () => new LocationViewModel(Context.Locations.Refresh(SelectedItem)));
            }
        }

        public class JobLocationListChildViewModel : ListViewModel<JobLocation>
        {
            protected override IEnumerable<Type> RefreshTypes
            {
                get
                {
                    yield return typeof(Location);
                    yield return typeof(JobLocation);
                }
            }

            public Job Job { get; private set; }

            public ICommand EditCommand { get; private set; }

            public JobLocationListChildViewModel(Job job)
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
                ServiceLocator.Document.Show(
                    model => model.Location.Equals(SelectedItem.Location),
                    () => new LocationViewModel(Context.Locations.Refresh(SelectedItem.Location)));
            }
        }

        public Job Job { get; private set; }
        public LocationListChildViewModel Locations { get; private set; }
        public JobLocationListChildViewModel JobLocations { get; private set; }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public JobLocationListViewModel(Job job)
        {
            Title = "Locations";
            Job = job;
            Locations = new LocationListChildViewModel(job);
            JobLocations = new JobLocationListChildViewModel(job);
            AddCommand = new Command(Add, Locations.HasAnySelectedItems);
            RemoveCommand = new AsyncCommand(RemoveAsync, JobLocations.HasAnySelectedItems);
            RefreshCommand = new Command(Refresh);
        }

        public void Add()
        {
            using (ServiceLocator.Busy.Begin())
            {
                foreach (Location location in Locations.SelectedItems)
                {
                    Context.JobLocations.Save(new JobLocation(true)
                    {
                        JobId = Job.JobId,
                        LocationId = location.LocationId
                    });
                }
            }
            ServiceLocator.Data.Refresh(typeof(JobLocation));
        }

        public async Task RemoveAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.JobLocationConfirmRemove, "Remove"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (JobLocation jobLocation in JobLocations.SelectedItems)
                    {
                        Context.JobLocations.Delete(jobLocation);
                    }
                }
                ServiceLocator.Data.Refresh(typeof(JobLocation));
            }
        }

        public void Refresh()
        {
            Locations.Refresh();
            JobLocations.Refresh();
        }
    }
}
