using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderReportViewModel : DocumentViewModel
    {
        public Responder Responder { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { SetProperty(nameof(Active), ref active, value); }
        }

        private ICollection<Incident> incidents;
        public ICollection<Incident> Incidents
        {
            get { return incidents; }
            set { SetProperty(nameof(Incidents), ref incidents, value); }
        }

        private ICollection<TeamResponder> teamResponders;
        public ICollection<TeamResponder> TeamResponders
        {
            get { return teamResponders; }
            set { SetProperty(nameof(TeamResponders), ref teamResponders, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            set { SetProperty(nameof(JobTickets), ref jobTickets, value); }
        }

        private ICollection<Record> records;
        public ICollection<Record> Records
        {
            get { return records; }
            set { SetProperty(nameof(Records), ref records, value); }
        }

        public ICommand EditIncidentCommand { get; private set; }
        public ICommand EditTeamCommand { get; private set; }
        public ICommand EditJobCommand { get; private set; }
        public ICommand EditRecordCommand { get; private set; }

        public ResponderReportViewModel(IServiceManager services, Responder responder)
            : base(services)
        {
            Title = "Reports";
            Responder = responder;
            EditIncidentCommand = new Command<Incident>(EditIncident);
            EditTeamCommand = new Command<TeamResponder>(EditTeam);
            EditJobCommand = new Command<JobTicket>(EditJob);
            EditRecordCommand = new AsyncCommand<Record>(EditRecordAsync);
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
            Incidents = null;
            TeamResponders = null;
            JobTickets = null;
            Records = null;
        }

        public async Task GenerateAsync()
        {
            Incidents = await TaskEx.Run(() => GetIncidents());
            TeamResponders = await TaskEx.Run(() => GetTeamResponders());
            JobTickets = await TaskEx.Run(() => GetJobTickets());
            Records = await TaskEx.Run(() => GetRecords());
        }

        private ICollection<Incident> GetIncidents()
        {
            return Context.Incidents.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(incident => incident.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private ICollection<TeamResponder> GetTeamResponders()
        {
            return Context.TeamResponders.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(teamResponder => teamResponder.Team.Incident.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(teamResponder => teamResponder.Team.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private ICollection<JobTicket> GetJobTickets()
        {
            return Context.JobTickets.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(jobTicket => jobTicket.Incident.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobTicket => jobTicket.Job.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(jobTicket => jobTicket.Team?.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private ICollection<Record> GetRecords()
        {
            return Context.Records.SelectByResponderId(Responder.ResponderId)
                .OrderBy(record => record.View.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.CreatedOn)
                .ToList();
        }

        public void EditIncident(Incident incident)
        {
            Services.Document.Show(
                model => model.Incident.Equals(incident),
                () => new IncidentDetailViewModel(Services, Context.Incidents.Refresh(incident)));
        }

        public void EditTeam(TeamResponder teamResponder)
        {
            Services.Document.Show(
                model => model.Team.Equals(teamResponder.Team),
                () => new TeamViewModel(Services, Context.Teams.Refresh(teamResponder.Team)));
        }

        public void EditJob(JobTicket jobTicket)
        {
            Services.Document.Show(
                model => model.Job.Equals(jobTicket.Job),
                () => new JobViewModel(Services, Context.Jobs.Refresh(jobTicket.Job)));
        }

        public async Task EditRecordAsync(Record record)
        {
            await Services.Wrapper.InvokeAsync(Enter.OpenRecord.Create(Context.Project.FilePath, record.View.Name, record.UniqueKey.Value));
        }
    }
}
