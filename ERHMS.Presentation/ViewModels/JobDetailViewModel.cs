using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobDetailViewModel : ViewModelBase
    {
        public class JobLocationListViewModel : ListViewModelBase<JobLocationViewModel>
        {
            public Job Job { get; private set; }

            public JobLocationListViewModel(Job job)
            {
                Job = job;
                Messenger.Default.Register<RefreshMessage<JobLocation>>(this, msg => Refresh());
            }

            protected override IEnumerable<JobLocationViewModel> GetItems()
            {
                ICollection<Location> locations = DataContext.Locations.SelectByJobId(Job.JobId).ToList();
                foreach (JobLocation jobLocation in DataContext.JobLocations.SelectByJobId(Job.JobId))
                {
                    Location itemLocation = locations.Single(location => location.LocationId.EqualsIgnoreCase(jobLocation.LocationId));
                    yield return new JobLocationViewModel(jobLocation, itemLocation);
                }
            }
        }

        public class JobTeamListViewModel : ListViewModelBase<JobTeamViewModel>
        {
            public Job Job { get; private set; }

            public JobTeamListViewModel(Job job)
            {
                Job = job;
                Messenger.Default.Register<RefreshMessage<JobTeam>>(this, msg => Refresh());
            }

            protected override IEnumerable<JobTeamViewModel> GetItems()
            {
                ICollection<Team> teams = DataContext.Teams.SelectByJobId(Job.JobId).ToList();
                foreach (JobTeam jobTeam in DataContext.JobTeams.SelectByJobId(Job.JobId))
                {
                    Team itemTeam = teams.Single(team => team.TeamId.EqualsIgnoreCase(jobTeam.TeamId));
                    yield return new JobTeamViewModel(jobTeam, itemTeam);
                }
            }
        }

        public Job Job { get; private set; }
        public JobLocationListViewModel Locations { get; private set; }
        public JobTeamListViewModel Teams { get; private set; }

        private ICollection<JobNote> notes;
        public ICollection<JobNote> Notes
        {
            get { return notes; }
            private set { Set(nameof(Notes), ref notes, value); }
        }

        private JobNote note;
        public JobNote Note
        {
            get { return note; }
            private set { Set(nameof(Note), ref note, value); }
        }

        public RelayCommand SaveJobCommand { get; private set; }
        public RelayCommand AddLocationCommand { get; private set; }
        public RelayCommand<JobLocationViewModel> RemoveLocationCommand { get; private set; }
        public RelayCommand AddTeamCommand { get; private set; }
        public RelayCommand<JobTeamViewModel> RemoveTeamCommand { get; private set; }
        public RelayCommand SaveNoteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public JobDetailViewModel(Job job)
        {
            Job = job;
            AddDirtyCheck(job);
            Refresh();
            Locations = new JobLocationListViewModel(job);
            Teams = new JobTeamListViewModel(job);
            Locations.Refresh();
            Teams.Refresh();
            ResetNote();
            RefreshNotes();
            SaveJobCommand = new RelayCommand(SaveJob);
            AddLocationCommand = new RelayCommand(AddLocation);
            RemoveLocationCommand = new RelayCommand<JobLocationViewModel>(RemoveLocation);
            AddTeamCommand = new RelayCommand(AddTeam);
            RemoveTeamCommand = new RelayCommand<JobTeamViewModel>(RemoveTeam);
            SaveNoteCommand = new RelayCommand(SaveNote, HasContent);
            RefreshCommand = new RelayCommand(RefreshAll);
            Messenger.Default.Register<RefreshMessage<JobNote>>(this, msg => RefreshNotes());
        }

        private void Refresh()
        {
            Title = Job.New ? "New Job" : Job.Name;
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Job.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                ShowRequiredMessage(fields);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SaveJob()
        {
            if (!Validate())
            {
                return;
            }
            DataContext.Jobs.Save(Job);
            Dirty = false;
            Messenger.Default.Send(new ToastMessage
            {
                Message = "Job has been saved."
            });
            Messenger.Default.Send(new RefreshMessage<Job>());
            Refresh();
        }

        public void AddLocation()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new JobLocationViewModel(Job)
                {
                    Active = true
                }
            });
        }

        public void RemoveLocation(JobLocationViewModel jobLocation)
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected location?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.JobLocations.Delete(jobLocation.JobLocation);
                Messenger.Default.Send(new RefreshMessage<JobLocation>());
            };
            Messenger.Default.Send(msg);
        }

        public void AddTeam()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new JobTeamViewModel(Job)
                {
                    Active = true
                }
            });
        }

        public void RemoveTeam(JobTeamViewModel jobTeam)
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Remove",
                Message = "Remove the selected team?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.JobTeams.Delete(jobTeam.JobTeam);
                Messenger.Default.Send(new RefreshMessage<JobTeam>());
            };
            Messenger.Default.Send(msg);
        }

        private bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Note.Content);
        }

        private void ResetNote()
        {
            if (Note != null)
            {
                RemoveDirtyCheck(Note);
            }
            Note = DataContext.JobNotes.Create();
            Note.JobId = Job.JobId;
            Note.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(JobNote.Content))
                {
                    SaveNoteCommand.RaiseCanExecuteChanged();
                }
            };
            AddDirtyCheck(Note);
        }

        public void SaveNote()
        {
            Note.Date = DateTime.Now;
            DataContext.JobNotes.Save(Note);
            Dirty = false;
            Messenger.Default.Send(new RefreshMessage<JobNote>());
            ResetNote();
        }

        private void RefreshNotes()
        {
            Notes = DataContext.JobNotes.SelectByJobId(Job.JobId)
                .OrderByDescending(note => note.Date)
                .ToList();
        }

        public void RefreshAll()
        {
            Locations.Refresh();
            Teams.Refresh();
            RefreshNotes();
        }
    }
}
