using ERHMS.Domain;
using ERHMS.Presentation.Services;

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

        public IncidentViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Incident = incident;
            Detail = new IncidentDetailViewModel(services, incident);
            Notes = new IncidentNoteListViewModel(services, incident);
            Rosters = new RosterListViewModel(services, incident);
            Roles = new IncidentRoleListViewModel(services, incident);
            Teams = new TeamListViewModel(services, incident);
            Locations = new LocationListViewModel(services, incident);
            Jobs = new JobListViewModel(services, incident);
            Views = new ViewListViewModel(services, incident);
            Templates = new TemplateListViewModel(services, incident);
            Assignments = new AssignmentListViewModel(services, incident);
            Pgms = new PgmListViewModel(services, incident);
            Canvases = new CanvasListViewModel(services, incident);
            Report = new IncidentReportViewModel(services, incident);
            Title = Detail.Title;
            Detail.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Title))
                {
                    Title = Detail.Title;
                }
            };
        }

        public override void Dispose()
        {
            Detail.Dispose();
            Notes.Dispose();
            Rosters.Dispose();
            Roles.Dispose();
            Teams.Dispose();
            Locations.Dispose();
            Jobs.Dispose();
            Views.Dispose();
            Templates.Dispose();
            Assignments.Dispose();
            Pgms.Dispose();
            Canvases.Dispose();
            Report.Dispose();
            base.Dispose();
        }
    }
}
