using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobLocationViewModel : ViewModelBase
    {
        public Job Job { get; private set; }
        public JobLocation JobLocation { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private ICollection<Location> locations;
        public ICollection<Location> Locations
        {
            get { return locations; }
            set { Set(nameof(Locations), ref locations, value); }
        }

        private Location location;
        public Location Location
        {
            get { return location; }
            set { Set(nameof(Location), ref location, value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public JobLocationViewModel(Job job)
        {
            Job = job;
            ICollection<string> locationIds = DataContext.JobLocations.SelectByJobId(job.JobId)
                .Select(jobLocation => jobLocation.LocationId)
                .ToList();
            Locations = DataContext.Locations.SelectByIncidentId(job.IncidentId)
                .Where(location => !locationIds.ContainsIgnoreCase(location.LocationId))
                .OrderBy(location => location.Name)
                .ToList();
            AddCommand = new RelayCommand(Add, HasLocation);
            CancelCommand = new RelayCommand(Cancel);
        }

        public JobLocationViewModel(JobLocation jobLocation, Location location)
        {
            JobLocation = jobLocation;
            Location = location;
        }

        public bool HasLocation()
        {
            return Location != null;
        }

        public void Add()
        {
            JobLocation jobLocation = DataContext.JobLocations.Create();
            jobLocation.JobId = Job.JobId;
            jobLocation.LocationId = Location.LocationId;
            DataContext.JobLocations.Save(jobLocation);
            Messenger.Default.Send(new RefreshMessage<JobLocation>());
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
