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
    public class IncidentDetailViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }
        public ICollection<Phase> Phases { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public IncidentDetailViewModel(Incident incident)
        {
            Title = incident.Name;
            Incident = incident;
            Phases = EnumExtensions.GetValues<Phase>().ToList();
            SaveCommand = new RelayCommand(Save);
        }

        private bool ValidateDates(DateTime? date1, DateTime? date2)
        {
            return !date1.HasValue || !date2.HasValue || date2.Value >= date1.Value;
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Incident.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                NotifyRequired(fields);
                return false;
            }
            else if (!ValidateDates(Incident.StartDate, Incident.EndDateEstimate))
            {
                Messenger.Default.Send(new NotifyMessage("End Date (Estimate) must be later than Start Date."));
                return false;
            }
            else if (!ValidateDates(Incident.StartDate, Incident.EndDateActual))
            {
                Messenger.Default.Send(new NotifyMessage("End Date (Actual) must be later than Start Date."));
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            DataContext.Incidents.Save(Incident);
            Messenger.Default.Send(new ToastMessage("Incident has been saved."));
            Messenger.Default.Send(new RefreshMessage<Incident>(Incident));
            Messenger.Default.Send(new RefreshListMessage<Incident>());
        }
    }
}
