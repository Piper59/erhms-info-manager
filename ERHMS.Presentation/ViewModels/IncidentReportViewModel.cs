﻿using ERHMS.DataAccess;
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
            set { SetProperty(nameof(IncidentNotes), ref incidentNotes, value); }
        }

        private ICollection<Job> jobs;
        public ICollection<Job> Jobs
        {
            get { return jobs; }
            set { SetProperty(nameof(Jobs), ref jobs, value); }
        }

        private ICollection<JobNote> jobNotes;
        public ICollection<JobNote> JobNotes
        {
            get { return jobNotes; }
            set { SetProperty(nameof(JobNotes), ref jobNotes, value); }
        }

        private ICollection<JobTeam> jobTeams;
        public ICollection<JobTeam> JobTeams
        {
            get { return jobTeams; }
            set { SetProperty(nameof(JobTeams), ref jobTeams, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            set { SetProperty(nameof(JobTickets), ref jobTickets, value); }
        }

        public ICommand EditJobCommand { get; private set; }
        public ICommand EditTeamCommand { get; private set; }
        public ICommand EditResponderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        public IncidentReportViewModel(IServiceManager services, Incident incident)
            : base(services)
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
            IncidentNotes = await TaskEx.Run(() => GetIncidentNotes());
            Jobs = await TaskEx.Run(() => GetJobs());
            JobNotes = await TaskEx.Run(() => GetJobNotes());
            JobTeams = await TaskEx.Run(() => GetJobTeams());
            JobTickets = await TaskEx.Run(() => GetJobTickets());
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
            Services.Document.Show(
                model => model.Job.Equals(job),
                () => new JobViewModel(Services, Context.Jobs.Refresh(job)));
        }

        public void EditTeam(JobTeam jobTeam)
        {
            Services.Document.Show(
                model => model.Team.Equals(jobTeam.Team),
                () => new TeamViewModel(Services, Context.Teams.Refresh(jobTeam.Team)));
        }

        public void EditResponder(JobTicket jobTicket)
        {
            Services.Document.Show(
                model => model.Responder.Equals(jobTicket.Responder),
                () => new ResponderViewModel(Services, Context.Responders.Refresh(jobTicket.Responder)));
        }

        public async Task RefreshAsync()
        {
            Clear();
            await GenerateAsync();
        }
    }
}
