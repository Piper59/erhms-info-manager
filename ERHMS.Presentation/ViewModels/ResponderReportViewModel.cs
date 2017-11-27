using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderReportViewModel : ViewModelBase
    {
        public Responder Responder { get; private set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { Set(nameof(IsSelected), ref isSelected, value); }
        }

        private ICollection<Incident> incidents;
        public ICollection<Incident> Incidents
        {
            get { return incidents; }
            set { Set(nameof(Incidents), ref incidents, value); }
        }

        private ICollection<TeamResponder> teamResponders;
        public ICollection<TeamResponder> TeamResponders
        {
            get { return teamResponders; }
            set { Set(nameof(TeamResponders), ref teamResponders, value); }
        }

        private ICollection<JobTicket> jobTickets;
        public ICollection<JobTicket> JobTickets
        {
            get { return jobTickets; }
            set { Set(nameof(JobTickets), ref jobTickets, value); }
        }

        private ICollection<Record> responses;
        public ICollection<Record> Responses
        {
            get { return responses; }
            set { Set(nameof(Responses), ref responses, value); }
        }

        public RelayCommand<Incident> EditIncidentCommand { get; private set; }
        public RelayCommand<TeamResponder> EditTeamCommand { get; private set; }
        public RelayCommand<JobTicket> EditJobCommand { get; private set; }
        public RelayCommand<Record> EditResponseCommand { get; private set; }

        public ResponderReportViewModel(IServiceManager services, Responder responder)
                : base(services)
        {
            Title = "Reports";
            Responder = responder;
            EditIncidentCommand = new RelayCommand<Incident>(EditIncident);
            EditTeamCommand = new RelayCommand<TeamResponder>(EditTeam);
            EditJobCommand = new RelayCommand<JobTicket>(EditJob);
            EditResponseCommand = new RelayCommand<Record>(EditResponse);
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
            Incidents = await TaskEx.Run(() => Context.Incidents.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(incident => incident.Name)
                .ToList());
            TeamResponders = await TaskEx.Run(() => Context.TeamResponders.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(teamResponder => teamResponder.Team.Incident.Name)
                .ThenBy(teamResponder => teamResponder.Team.Name)
                .ToList());
            JobTickets = await TaskEx.Run(() => Context.JobTickets.SelectUndeletedByResponderId(Responder.ResponderId)
                .OrderBy(jobTicket => jobTicket.Incident.Name)
                .ThenBy(jobTicket => jobTicket.Job.Name)
                .ThenBy(jobTicket => jobTicket.Team?.Name)
                .ToList());
            Responses = await TaskEx.Run(() => Context.Responses.SelectByResponderId(Responder.ResponderId)
                .OrderBy(response => response.View.Name)
                .ThenBy(response => response.CreatedOn)
                .ToList());
        }

        public void Clear()
        {
            Incidents = null;
            TeamResponders = null;
            JobTickets = null;
            Responses = null;
        }

        public void EditIncident(Incident incident)
        {
            Documents.ShowIncident(incident);
        }

        public void EditTeam(TeamResponder teamResponder)
        {
            Documents.ShowTeam(teamResponder.Team);
        }

        public void EditJob(JobTicket jobTicket)
        {
            Documents.ShowJob(jobTicket.Job);
        }

        public void EditResponse(Record response)
        {
            Dialogs.InvokeAsync(Enter.OpenRecord.Create(Context.Project.FilePath, response.View.Name, response.UniqueKey.Value));
        }
    }
}
