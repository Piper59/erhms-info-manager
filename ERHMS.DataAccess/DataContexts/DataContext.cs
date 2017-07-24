using Epi;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using System.Data;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.DataAccess
{
    public class DataContext : DataContextBase
    {
        public static void Configure()
        {
            AssignmentRepository.Configure();
            CanvasRepository.Configure();
            CanvasLinkRepository.Configure();
            IncidentRepository.Configure();
            IncidentNoteRepository.Configure();
            LocationRepository.Configure();
            PgmRepository.Configure();
            PgmLinkRepository.Configure();
            ResponderRepository.Configure();
            RosterRepository.Configure();
            ViewRepository.Configure();
            ViewLinkRepository.Configure();
            WebSurveyRepository.Configure();
        }

        public static DataContext Create(Project project)
        {
            Log.Logger.DebugFormat("Creating data context: {0}", project.FilePath);
            string path = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.DataAccess.Resources.Project.xml", path);
            TemplateInfo templateInfo = TemplateInfo.Get(path);
            Wrapper wrapper = MakeView.InstantiateProjectTemplate.Create(project.FilePath, path);
            wrapper.Invoke();
            wrapper.Exited.WaitOne();
            foreach (View view in project.Views)
            {
                project.CollectedData.CreateDataTableForView(view, 1);
            }
            DataContext context = new DataContext(project);
            using (IDbConnection connection = context.Database.GetConnection())
            {
                connection.Execute(new Script(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.DataAccess.Scripts.Create.sql")));
            }
            return context;
        }

        public AssignmentRepository Assignments { get; private set; }
        public CanvasRepository Canvases { get; private set; }
        public CanvasLinkRepository CanvasLinks { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public IncidentNoteRepository IncidentNotes { get; private set; }
        public LocationRepository Locations { get; private set; }
        public PgmRepository Pgms { get; private set; }
        public PgmLinkRepository PgmLinks { get; private set; }
        public ResponderRepository Responders { get; private set; }
        public RosterRepository Rosters { get; private set; }
        public ViewRepository Views { get; private set; }
        public ViewLinkRepository ViewLinks { get; private set; }
        public WebSurveyRepository WebSurveys { get; private set; }

        public DataContext(Project project)
            : base(project)
        {
            Assignments = new AssignmentRepository(this);
            Canvases = new CanvasRepository(this);
            CanvasLinks = new CanvasLinkRepository(this);
            Incidents = new IncidentRepository(this);
            IncidentNotes = new IncidentNoteRepository(this);
            Locations = new LocationRepository(this);
            Pgms = new PgmRepository(this);
            PgmLinks = new PgmLinkRepository(this);
            Responders = new ResponderRepository(this);
            Rosters = new RosterRepository(this);
            Views = new ViewRepository(this);
            ViewLinks = new ViewLinkRepository(this);
            WebSurveys = new WebSurveyRepository(this);
        }
    }
}
