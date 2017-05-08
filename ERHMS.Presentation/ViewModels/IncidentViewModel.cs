using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }
        public IncidentDetailViewModel Detail { get; private set; }
        public IncidentNotesViewModel Notes { get; private set; }
        public RosterListViewModel Rosters { get; private set; }
        public TeamListViewModel Teams { get; private set; }
        public LocationListViewModel Locations { get; private set; }
        public JobListViewModel Jobs { get; private set; }
        public ViewListViewModel Views { get; private set; }
        public TemplateListViewModel Templates { get; private set; }
        public AssignmentListViewModel Assignments { get; private set; }
        public PgmListViewModel Pgms { get; private set; }
        public CanvasListViewModel Canvases { get; private set; }

        public override bool Dirty
        {
            get { return base.Dirty || Detail.Dirty || Notes.Dirty; }
            protected set { base.Dirty = value; }
        }

        public IncidentViewModel(Incident incident)
        {
            Incident = incident;
            Refresh();
            Detail = new IncidentDetailViewModel(incident);
            Notes = new IncidentNotesViewModel(incident);
            Rosters = new RosterListViewModel(incident);
            Teams = new TeamListViewModel(incident);
            Locations = new LocationListViewModel(incident);
            Jobs = new JobListViewModel(incident);
            Views = new ViewListViewModel(incident);
            Templates = new TemplateListViewModel(incident);
            Assignments = new AssignmentListViewModel(incident);
            Pgms = new PgmListViewModel(incident);
            Canvases = new CanvasListViewModel(incident);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, msg => Refresh());
        }

        private void Refresh()
        {
            Title = Incident.New ? "New Incident" : Incident.Name;
        }
    }
}
