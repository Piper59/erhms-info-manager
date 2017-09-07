using ERHMS.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderDetailViewModel : ViewModelBase
    {
        public class ReportChildViewModel : ViewModelBase
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

            private ICollection<ResponderViewEntity> records;
            public ICollection<ResponderViewEntity> Records
            {
                get { return records; }
                set { Set(nameof(Records), ref records, value); }
            }

            public RelayCommand<Incident> EditIncidentCommand { get; private set; }
            public RelayCommand<TeamResponder> EditTeamCommand { get; private set; }
            public RelayCommand<JobTicket> EditJobCommand { get; private set; }
            public RelayCommand<ResponderViewEntity> EditRecordCommand { get; private set; }

            public ReportChildViewModel(IServiceManager services, Responder responder)
                : base(services)
            {
                Responder = responder;
                EditIncidentCommand = new RelayCommand<Incident>(EditIncident);
                EditTeamCommand = new RelayCommand<TeamResponder>(EditTeam);
                EditJobCommand = new RelayCommand<JobTicket>(EditJob);
                EditRecordCommand = new RelayCommand<ResponderViewEntity>(EditRecord);
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(IsSelected))
                    {
                        if (IsSelected)
                        {
                            GenerateAsync();
                        }
                    }
                };
            }

            public async void GenerateAsync()
            {
                await TaskEx.Run(() =>
                {
                    Incidents = Context.Incidents.SelectUndeletedByResponderId(Responder.ResponderId)
                        .OrderBy(incident => incident.Name)
                        .ToList();
                    TeamResponders = Context.TeamResponders.SelectUndeletedByResponderId(Responder.ResponderId)
                        .OrderBy(teamResponder => teamResponder.Team.Incident.Name)
                        .ThenBy(teamResponder => teamResponder.Team.Name)
                        .ToList();
                    JobTickets = Context.JobTickets.SelectUndeletedByResponderId(Responder.ResponderId)
                        .OrderBy(jobTicket => jobTicket.Incident.Name)
                        .ThenBy(jobTicket => jobTicket.Job.Name)
                        .ThenBy(jobTicket => jobTicket.Team?.Name)
                        .ToList();
                    Records = Context.ResponderViewEntities.SelectByResponderId(Responder.ResponderId)
                        .OrderBy(record => record.View.Name)
                        .ThenBy(record => record.CreatedOn)
                        .ToList();
                });
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

            public void EditRecord(ResponderViewEntity record)
            {
                Dialogs.InvokeAsync(Enter.OpenRecord.Create(Context.Project.FilePath, record.View.Name, record.UniqueKey.Value));
            }
        }

        public Responder Responder { get; private set; }
        public ICollection<string> Prefixes { get; private set; }
        public ICollection<string> Suffixes { get; private set; }
        public ICollection<string> Genders { get; private set; }
        public ICollection<string> States { get; private set; }
        public ReportChildViewModel Report { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public ResponderDetailViewModel(IServiceManager services, Responder responder)
            : base(services)
        {
            Title = responder.New ? "New Responder" : responder.FullName;
            Responder = responder;
            Prefixes = Context.Prefixes.ToList();
            Suffixes = Context.Suffixes.ToList();
            Genders = Context.Genders.ToList();
            States = Context.States.ToList();
            Report = new ReportChildViewModel(services, responder);
            SaveCommand = new RelayCommand(Save);
            AddDirtyCheck(responder);
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Responder.FirstName))
            {
                fields.Add("First Name");
            }
            if (string.IsNullOrWhiteSpace(Responder.LastName))
            {
                fields.Add("Last Name");
            }
            if (string.IsNullOrWhiteSpace(Responder.EmailAddress))
            {
                fields.Add("Email Address");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (Responder.BirthDate.HasValue && Responder.BirthDate.Value.Date > DateTime.Today)
            {
                fields.Add("Birth Date");
            }
            if (!MailExtensions.IsValidAddress(Responder.EmailAddress))
            {
                fields.Add("Email Address");
            }
            if (!string.IsNullOrWhiteSpace(Responder.ContactEmailAddress) && !MailExtensions.IsValidAddress(Responder.ContactEmailAddress))
            {
                fields.Add("Emergency Contact Email Address");
            }
            if (!string.IsNullOrWhiteSpace(Responder.OrganizationEmailAddress) && !MailExtensions.IsValidAddress(Responder.OrganizationEmailAddress))
            {
                fields.Add("Organization Email Address");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Context.Responders.Save(Responder);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Responder has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Responder)));
            Title = Responder.FullName;
            Dirty = false;
        }
    }
}
