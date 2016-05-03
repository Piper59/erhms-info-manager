using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }
        public IncidentDetailViewModel Detail { get; private set; }
        public IncidentNotesViewModel Notes { get; private set; }
        public LocationListViewModel Locations { get; private set; }
        public ViewListViewModel Views { get; private set; }
        public TemplateListViewModel Templates { get; private set; }
        public PgmListViewModel Pgms { get; private set; }
        public CanvasListViewModel Canvases { get; private set; }

        public IncidentViewModel(Incident incident)
        {
            Incident = incident;
            incident.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "New")
                {
                    UpdateTitle();
                }
            };
            UpdateTitle();
            Detail = new IncidentDetailViewModel(incident);
            Notes = new IncidentNotesViewModel(incident);
            Locations = new LocationListViewModel(incident);
            Views = new ViewListViewModel(incident);
            Templates = new TemplateListViewModel(incident);
            Pgms = new PgmListViewModel(incident);
            Canvases = new CanvasListViewModel(incident);
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
