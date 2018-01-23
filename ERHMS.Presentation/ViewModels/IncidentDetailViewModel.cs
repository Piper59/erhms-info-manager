using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentDetailViewModel : DocumentViewModel
    {
        public Incident Incident { get; private set; }
        public ICollection<Phase> Phases { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public IncidentDetailViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = incident.New ? "New Incident" : incident.Name;
            Incident = incident;
            AddDirtyCheck(incident);
            Phases = EnumExtensions.GetValues<Phase>().ToList();
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Incident.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                await Services.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Incident.StartDate, Incident.EndDateEstimate))
            {
                await Services.Dialog.AlertAsync("Estimated end date must be later than start date.");
                return false;
            }
            if (!DateTimeExtensions.AreInOrder(Incident.StartDate, Incident.EndDateActual))
            {
                await Services.Dialog.AlertAsync("Actual end date must be later than start date.");
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            bool @new = Incident.New;
            Context.Incidents.Save(Incident);
            Services.Dialog.Notify("Incident has been saved.");
            Services.Data.Refresh(typeof(Incident));
            if (@new)
            {
                Services.Data.Refresh(typeof(IncidentRole));
            }
            Title = Incident.Name;
            Dirty = false;
        }
    }
}
