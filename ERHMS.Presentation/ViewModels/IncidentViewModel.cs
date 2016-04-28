using ERHMS.Domain;
using System.ComponentModel;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentViewModel : DocumentViewModel
    {
        public Incident Incident { get; private set; }
        public IncidentDetailViewModel Detail { get; private set; }

        public IncidentViewModel(Incident incident)
        {
            Incident = incident;
            Incident.PropertyChanged += Incident_PropertyChanged;
            UpdateTitle();
            Detail = new IncidentDetailViewModel(incident);
        }

        private void Incident_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Incident incident = (Incident)sender;
            switch (e.PropertyName)
            {
                case "New":
                    UpdateTitle();
                    break;
            }
        }

        private void UpdateTitle()
        {
            if (Incident.New)
            {
                Title = "New Incident";
            }
            else
            {
                Title = Incident.Name;
            }
        }
    }
}
