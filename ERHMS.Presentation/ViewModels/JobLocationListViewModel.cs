using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobLocationListViewModel : ListViewModel<JobLocation>
    {
        public class LocationListChildViewModel : ListViewModel<Location>
        {
            public Job Job { get; private set; }

            public RelayCommand EditCommand { get; private set; }

            public LocationListChildViewModel(IServiceManager services, Job job)
                : base(services)
            {
                Job = job;
                EditCommand = new RelayCommand(Edit, HasSelectedItem);
                SelectionChanged += (sender, e) =>
                {
                    EditCommand.RaiseCanExecuteChanged();
                };
            }

            protected override IEnumerable<Location> GetItems()
            {
                return Context.Locations.SelectJobbable(Job.IncidentId, Job.JobId).OrderBy(location => location.Name);
            }

            protected override IEnumerable<string> GetFilteredValues(Location item)
            {
                yield return item.Name;
            }

            public void Edit()
            {
                Documents.ShowLocation((Location)SelectedItem.Clone());
            }
        }

        public Job Job { get; private set; }
        public LocationListChildViewModel Locations { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }

        public JobLocationListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Locations";
            Job = job;
            Locations = new LocationListChildViewModel(services, job);
            Refresh();
            AddCommand = new RelayCommand(Add, Locations.HasSelectedItem);
            RemoveCommand = new RelayCommand(Remove, HasSelectedItem);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            Locations.SelectionChanged += (sender, e) =>
            {
                AddCommand.RaiseCanExecuteChanged();
            };
            SelectionChanged += (sender, e) =>
            {
                RemoveCommand.RaiseCanExecuteChanged();
                EditCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<JobLocation> GetItems()
        {
            return Context.JobLocations.SelectByJobId(Job.JobId).OrderBy(jobLocation => jobLocation.Location.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(JobLocation item)
        {
            yield return item.Location.Name;
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
            MessengerInstance.Send(new RefreshMessage(typeof(JobLocation)));
        }

        public void Remove()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected locations?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (JobLocation jobLocation in SelectedItems)
                {
                    Context.JobLocations.Delete(jobLocation);
                }
                MessengerInstance.Send(new RefreshMessage(typeof(JobLocation)));
            };
            MessengerInstance.Send(msg);
        }

        public void Edit()
        {
            Documents.ShowLocation((Location)SelectedItem.Location.Clone());
        }

        public override void Refresh()
        {
            Locations.Refresh();
            base.Refresh();
        }
    }
}
