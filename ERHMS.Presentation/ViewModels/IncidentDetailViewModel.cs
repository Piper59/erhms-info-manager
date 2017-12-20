using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentDetailViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }
        public ICollection<Phase> Phases { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public IncidentDetailViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = incident.New ? "New Incident" : incident.Name;
            Incident = incident;
            Phases = EnumExtensions.GetValues<Phase>().ToList();
            SaveCommand = new RelayCommand(Save);
            AddDirtyCheck(incident);
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
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Incident.StartDate, Incident.EndDateEstimate))
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Estimated end date must be later than start date."
                });
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Incident.StartDate, Incident.EndDateActual))
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Actual end date must be later than start date."
                });
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
            bool created = Incident.New;
            Context.Incidents.Save(Incident);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Incident has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Incident)));
            if (created)
            {
                MessengerInstance.Send(new RefreshMessage(typeof(IncidentRole)));
            }
            Title = Incident.Name;
            Dirty = false;
        }
    }
}
