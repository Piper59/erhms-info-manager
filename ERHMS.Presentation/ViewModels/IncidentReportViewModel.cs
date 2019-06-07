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
    public class IncidentReportViewModel : DocumentViewModel
    {
        private bool generating;

        public Incident Incident { get; private set; }

        private DateTime? startDate;
        public DateTime? StartDate
        {
            get { return startDate; }
            set { SetProperty(nameof(StartDate), ref startDate, value); }
        }

        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { SetProperty(nameof(EndDate), ref endDate, value); }
        }

        private ICollection<IncidentNote> incidentNotes;
        public ICollection<IncidentNote> IncidentNotes
        {
            get { return incidentNotes; }
            private set { SetProperty(nameof(IncidentNotes), ref incidentNotes, value); }
        }

        private ICollection<Job> jobs;
        public ICollection<Job> Jobs
        {
            get { return jobs; }
            private set { SetProperty(nameof(Jobs), ref jobs, value); }
        }

        private ICollection<JobNote> jobNotes;
        public ICollection<JobNote> JobNotes
        {
            get { return jobNotes; }
            private set { SetProperty(nameof(JobNotes), ref jobNotes, value); }
        }

        private ICollection<JobTeam> jobTeams;
        public ICollection<JobTeam> JobTeams
        {
            get { return jobTeams; }
            private set { SetProperty(nameof(JobTeams), ref jobTeams, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            private set { SetProperty(nameof(JobTickets), ref jobTickets, value); }
        }

        public ICommand EditJobCommand { get; private set; }
        public ICommand EditTeamCommand { get; private set; }
        public ICommand EditResponderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public IncidentReportViewModel(Incident incident)
        {
            Title = "Reports";
            Incident = incident;
            StartDate = DateTime.Now - TimeSpan.FromHours(12.0);
            EditJobCommand = new Command<Job>(EditJob);
            EditTeamCommand = new Command<JobTeam>(EditTeam);
            EditResponderCommand = new Command<JobTicket>(EditResponder);
            RefreshCommand = new AsyncCommand(RefreshAsync);
            PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == nameof(Active))
                {
                    if (generating)
                    {
                        return;
                    }
                    if (Active)
                    {
                        await GenerateAsync();
                    }
                    else
                    {
                        Clear();
                    }
                }
            };
        }

        public void Clear()
        {
            IncidentNotes = null;
            Jobs = null;
            JobNotes = null;
            JobTeams = null;
            JobTickets = null;
        }

        public async Task GenerateAsync()
        {
            try
            {
                generating = true;
                IncidentNotes = await Task.Run(() => GetIncidentNotes());
                Jobs = await Task.Run(() => GetJobs());
                JobNotes = await Task.Run(() => GetJobNotes());
                JobTeams = await Task.Run(() => GetJobTeams());
                JobTickets = await Task.Run(() => GetJobTickets());
            }
            finally
            {
                generating = false;
            }
        }

        private ICollection<IncidentNote> GetIncidentNotes()
        {
            return Context.IncidentNotes.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(incidentNote => incidentNote.Date)
                .ToList();
        }

        private ICollection<Job> GetJobs()
        {
            return Context.Jobs.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(job => job.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private ICollection<JobNote> GetJobNotes()
        {
            return Context.JobNotes.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(jobNote => jobNote.Job.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobNote => jobNote.Date)
                .ToList();
        }

        private ICollection<JobTeam> GetJobTeams()
        {
            return Context.JobTeams.SelectByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .OrderBy(jobTeam => jobTeam.Job.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobTeam => jobTeam.Team.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private ICollection<JobTicket> GetJobTickets()
        {
            return Context.JobTickets.SelectUndeletedByIncidentIdAndDateRange(Incident.IncidentId, StartDate, EndDate)
                .WithLocations(Context)
                .OrderBy(jobTicket => jobTicket.Job.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobTicket => jobTicket.Team?.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobTicket => jobTicket.Responder.FullName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public void EditJob(Job job)
        {
            ServiceLocator.Document.Show(
                model => model.Job.Equals(job),
                () => new JobViewModel(Context.Jobs.Refresh(job)));
        }

        public void EditTeam(JobTeam jobTeam)
        {
            ServiceLocator.Document.Show(
                model => model.Team.Equals(jobTeam.Team),
                () => new TeamViewModel(Context.Teams.Refresh(jobTeam.Team)));
        }

        public void EditResponder(JobTicket jobTicket)
        {
            ServiceLocator.Document.Show(
                model => model.Responder.Equals(jobTicket.Responder),
                () => new ResponderViewModel(Context.Responders.Refresh(jobTicket.Responder)));
        }

        public async Task RefreshAsync()
        {
            if (generating)
            {
                return;
            }
            Clear();
            await GenerateAsync();
        }
    }
}
