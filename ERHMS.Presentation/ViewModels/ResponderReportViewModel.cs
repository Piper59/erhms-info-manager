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
        private bool generating;

        public Responder Responder { get; private set; }

        private ICollection<Incident> incidents;
        public ICollection<Incident> Incidents
        {
            get { return incidents; }
            private set { SetProperty(nameof(Incidents), ref incidents, value); }
        }

        private ICollection<TeamResponder> teamResponders;
        public ICollection<TeamResponder> TeamResponders
        {
            get { return teamResponders; }
            private set { SetProperty(nameof(TeamResponders), ref teamResponders, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            private set { SetProperty(nameof(JobTickets), ref jobTickets, value); }
        }

        private ICollection<ResponderEntity> entities;
        public ICollection<ResponderEntity> Entities
        {
            get { return entities; }
            private set { SetProperty(nameof(Entities), ref entities, value); }
        }

        public ICommand EditIncidentCommand { get; private set; }
        public ICommand EditTeamCommand { get; private set; }
        public ICommand EditJobCommand { get; private set; }
        public ICommand EditEntityCommand { get; private set; }

        public ResponderReportViewModel(Responder responder)
        {
            Title = "Reports";
            Responder = responder;
            EditIncidentCommand = new Command<Incident>(EditIncident);
            EditTeamCommand = new Command<TeamResponder>(EditTeam);
            EditJobCommand = new Command<JobTicket>(EditJob);
            EditEntityCommand = new AsyncCommand<ResponderEntity>(EditEntityAsync);
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
            Incidents = null;
            TeamResponders = null;
            JobTickets = null;
            Entities = null;
        }

        public async Task GenerateAsync()
        {
            try
            {
                generating = true;
                Incidents = await Task.Run(() => GetIncidents());
                TeamResponders = await Task.Run(() => GetTeamResponders());
                JobTickets = await Task.Run(() => GetJobTickets());
                Entities = await Task.Run(() => GetEntities());
            }
            finally
            {
                generating = false;
            }
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

        private ICollection<ResponderEntity> GetEntities()
        {
            return Context.ResponderEntities.SelectByResponderId(Responder.ResponderId)
                .OrderBy(entity => entity.View.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(entity => entity.CreatedOn)
                .ToList();
        }

        public void EditIncident(Incident incident)
        {
            ServiceLocator.Document.Show(
                model => model.Incident.Equals(incident),
                () => new IncidentDetailViewModel(Context.Incidents.Refresh(incident)));
        }

        public void EditTeam(TeamResponder teamResponder)
        {
            ServiceLocator.Document.Show(
                model => model.Team.Equals(teamResponder.Team),
                () => new TeamViewModel(Context.Teams.Refresh(teamResponder.Team)));
        }

        public void EditJob(JobTicket jobTicket)
        {
            ServiceLocator.Document.Show(
                model => model.Job.Equals(jobTicket.Job),
                () => new JobViewModel(Context.Jobs.Refresh(jobTicket.Job)));
        }

        public async Task EditEntityAsync(ResponderEntity entity)
        {
            Wrapper wrapper = Enter.OpenRecord.Create(Context.Project.FilePath, entity.View.Name, entity.UniqueKey.Value);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }
    }
}
