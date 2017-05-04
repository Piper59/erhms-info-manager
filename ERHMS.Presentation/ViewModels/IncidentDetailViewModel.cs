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
            Incident = incident;
            AddDirtyCheck(incident);
            Phases = EnumExtensions.GetValues<Phase>().ToList();
            SaveCommand = new RelayCommand(Save);
        }

        private bool ValidateDates(DateTime? startDate, DateTime? endDate)
        {
            return !startDate.HasValue || !endDate.HasValue || endDate.Value >= startDate.Value;
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
                ShowRequiredMessage(fields);
                return false;
            }
            else if (!ValidateDates(Incident.StartDate, Incident.EndDateEstimate))
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Estimated end date must be later than start date."
                });
                return false;
            }
            else if (!ValidateDates(Incident.StartDate, Incident.EndDateActual))
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Actual end date must be later than start date."
                });
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
            Dirty = false;
            Messenger.Default.Send(new ToastMessage
            {
                Message = "Incident has been saved."
            });
            Messenger.Default.Send(new RefreshMessage<Incident>());
        }
    }
}
