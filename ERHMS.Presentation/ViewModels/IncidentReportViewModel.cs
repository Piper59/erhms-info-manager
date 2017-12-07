using ERHMS.DataAccess;
using ERHMS.Domain;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentReportViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(nameof(IsSelected), ref isSelected, value); }
        }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get { return startDate; }
            set { Set(nameof(StartDate), ref startDate, value); }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { Set(nameof(EndDate), ref endDate, value); }
        }

        private ICollection<IncidentNote> incidentNotes;
        public ICollection<IncidentNote> IncidentNotes
        {
            get { return incidentNotes; }
            set { Set(nameof(IncidentNotes), ref incidentNotes, value); }
        }

        private ICollection<Job> jobs;
        public ICollection<Job> Jobs
        {
            get { return jobs; }
            set { Set(nameof(Jobs), ref jobs, value); }
        }

        private ICollection<JobNote> jobNotes;
        public ICollection<JobNote> JobNotes
        {
            get { return jobNotes; }
            set { Set(nameof(JobNotes), ref jobNotes, value); }
        }

        private ICollection<JobTeam> jobTeams;
        public ICollection<JobTeam> JobTeams
        {
            get { return jobTeams; }
            set { Set(nameof(JobTeams), ref jobTeams, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            set { Set(nameof(JobTickets), ref jobTickets, value); }
        }

        public RelayCommand<Job> EditJobCommand { get; private set; }
        public RelayCommand<JobTeam> EditTeamCommand { get; private set; }
        public RelayCommand<JobTicket> EditResponderCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentReportViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Reports";
            Incident = incident;
            EndDate = DateTime.Now;
            StartDate = EndDate - TimeSpan.FromHours(12.0);
            EditJobCommand = new RelayCommand<Job>(EditJob);
            EditTeamCommand = new RelayCommand<JobTeam>(EditTeam);
            EditResponderCommand = new RelayCommand<JobTicket>(EditResponder);
            RefreshCommand = new RelayCommand(Refresh);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsSelected))
                {
                    if (IsSelected)
                    {
                        GenerateAsync();
                    }
                    else
                    {
                        Clear();
                    }
                }
            };
        }

        public async void GenerateAsync()
        {
            IncidentNotes = await TaskEx.Run(() => Context.IncidentNotes.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(incidentNote => incidentNote.Date)
                .ToList());
            Jobs = await TaskEx.Run(() => Context.Jobs.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(job => job.Name)
                .ToList());
            JobNotes = await TaskEx.Run(() => Context.JobNotes.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(jobNote => jobNote.Job.Name)
                .ThenBy(jobNote => jobNote.Date)
                .ToList());
            ICollectionView jobNotesView = CollectionViewSource.GetDefaultView(JobNotes);
            jobNotesView.GroupDescriptions.Add(new PropertyGroupDescription("Job"));
            JobTeams = await TaskEx.Run(() => Context.JobTeams.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(jobTeam => jobTeam.Job.Name)
                .ThenBy(jobTeam => jobTeam.Team.Name)
                .ToList());
            JobTickets = await TaskEx.Run(() => Context.JobTickets.SelectUndeletedByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .WithLocations(Context)
                .OrderBy(jobTicket => jobTicket.Job.Name)
                .ThenBy(jobTicket => jobTicket.Team?.Name)
                .ThenBy(jobTicket => jobTicket.Responder.FullName)
                .ToList());
        }

        public void Clear()
        {
            IncidentNotes = null;
            Jobs = null;
            JobNotes = null;
            JobTeams = null;
            JobTickets = null;
        }

        public void EditJob(Job job)
        {
            Documents.ShowJob(job);
        }

        public void EditTeam(JobTeam jobTeam)
        {
            Documents.ShowTeam(jobTeam.Team);
        }

        public void EditResponder(JobTicket jobTicket)
        {
            Documents.ShowResponder(jobTicket.Responder);
        }

        public void Refresh()
        {
            Clear();
            GenerateAsync();
        }
    }
}
