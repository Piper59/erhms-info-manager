using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentViewModel : DocumentViewModel
    {
        public Incident Incident { get; private set; }
        public IncidentDetailViewModel Detail { get; private set; }
        public IncidentNoteListViewModel Notes { get; private set; }
        public RosterListViewModel Rosters { get; private set; }
        public IncidentRoleListViewModel Roles { get; private set; }
        public TeamListViewModel Teams { get; private set; }
        public LocationListViewModel Locations { get; private set; }
        public JobListViewModel Jobs { get; private set; }
        public ViewListViewModel Views { get; private set; }
        public TemplateListViewModel Templates { get; private set; }
        public AssignmentListViewModel Assignments { get; private set; }
        public PgmListViewModel Pgms { get; private set; }
        public CanvasListViewModel Canvases { get; private set; }
        public IncidentReportViewModel Report { get; private set; }

        public override bool Dirty
        {
            get { return base.Dirty || Detail.Dirty || Notes.Dirty; }
            protected set { base.Dirty = value; }
        }

        public IncidentViewModel(Incident incident)
        {
            Incident = incident;
            Detail = new IncidentDetailViewModel(incident);
            Notes = new IncidentNoteListViewModel(incident);
            Rosters = new RosterListViewModel(incident);
            Roles = new IncidentRoleListViewModel(incident);
            Teams = new TeamListViewModel(incident);
            Locations = new LocationListViewModel(incident);
            Jobs = new JobListViewModel(incident);
            Views = new ViewListViewModel(incident);
            Templates = new TemplateListViewModel(incident);
            Assignments = new AssignmentListViewModel(incident);
            Pgms = new PgmListViewModel(incident);
            Canvases = new CanvasListViewModel(incident);
            Report = new IncidentReportViewModel(incident);
            Title = Detail.Title;
            Detail.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Title))
                {
                    Title = Detail.Title;
                }
            };
        }
    }
}
