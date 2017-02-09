using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.DataAccess
{
    // TODO: Add hook for version upgrade actions
    public class DataContext
    {
        public static DataContext Create(Project project)
        {
            Log.Current.DebugFormat("Creating data context: {0}", project.FilePath);
            FileInfo templateFile = IOExtensions.GetTemporaryFile("ERHMS_{0:N}.xml");
            Assembly.GetAssembly(typeof(Responder)).CopyManifestResourceTo("ERHMS.Domain.Templates.Projects.ERHMS.xml", templateFile);
            TemplateInfo templateInfo = TemplateInfo.Get(templateFile);
            Wrapper wrapper = MakeView.InstantiateTemplate(project, templateInfo);
            wrapper.Invoke();
            wrapper.Exited.WaitOne();
            foreach (View view in project.Views)
            {
                project.CollectedData.CreateDataTableForView(view, 1);
            }
            IDataDriver driver = DataDriverFactory.CreateDataDriver(project);
            driver.ExecuteScript(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.DataAccess.Scripts.Create.sql"));
            return new DataContext(project);
        }

        internal static DataPredicate GetIncidentPredicate(IDataDriver driver, string incidentId)
        {
            DataParameterCollection parameters = new DataParameterCollection(driver);
            parameters.AddByValue(incidentId);
            string sql = parameters.Format("[IncidentId] = {0}");
            return new DataPredicate(sql, parameters);
        }

        public Project Project { get; private set; }
        public IDataDriver Driver { get; private set; }
        public CodeRepository Prefixes { get; private set; }
        public CodeRepository Suffixes { get; private set; }
        public CodeRepository Genders { get; private set; }
        public CodeRepository States { get; private set; }
        public ResponderRepository Responders { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public IncidentNoteRepository IncidentNotes { get; private set; }
        public RosterRepository Rosters { get; private set; }
        public LocationRepository Locations { get; private set; }
        public AssignmentRepository Assignments { get; private set; }
        public ViewLinkRepository ViewLinks { get; private set; }
        public PgmLinkRepository PgmLinks { get; private set; }
        public CanvasLinkRepository CanvasLinks { get; private set; }
        public WebSurveyRepository WebSurveys { get; private set; }

        public DataContext(Project project)
        {
            Log.Current.DebugFormat("Opening data context: {0}", project.FilePath);
            Project = project;
            Driver = DataDriverFactory.CreateDataDriver(project);
            Prefixes = new CodeRepository(Driver, "codeprefix1", "prefix", false);
            Suffixes = new CodeRepository(Driver, "codesuffix1", "suffix", false);
            Genders = new CodeRepository(Driver, "codegender1", "gender", false);
            States = new CodeRepository(Driver, "codestate1", "state", true);
            Responders = new ResponderRepository(Driver, project);
            Incidents = new IncidentRepository(Driver);
            IncidentNotes = new IncidentNoteRepository(Driver);
            Rosters = new RosterRepository(Driver);
            Locations = new LocationRepository(Driver);
            Assignments = new AssignmentRepository(Driver);
            ViewLinks = new ViewLinkRepository(Driver);
            PgmLinks = new PgmLinkRepository(Driver);
            CanvasLinks = new CanvasLinkRepository(Driver);
            WebSurveys = new WebSurveyRepository(Driver);
        }

        private DataPredicate GetIncidentPredicate(string incidentId)
        {
            return GetIncidentPredicate(Driver, incidentId);
        }

        public IEnumerable<View> GetViews()
        {
            return Project.GetViews();
        }

        public IEnumerable<TemplateInfo> GetTemplateInfos(TemplateLevel? level = null)
        {
            if (level.HasValue)
            {
                return TemplateInfo.GetByLevel(level.Value);
            }
            else
            {
                return TemplateInfo.GetAll();
            }
        }

        public IEnumerable<Pgm> GetPgms()
        {
            return Project.GetPgms();
        }

        public IEnumerable<Canvas> GetCanvases()
        {
            return Project.GetCanvases();
        }

        public IEnumerable<Link<View>> GetLinkedViews()
        {
            ICollection<Incident> incidents = Incidents.Select().ToList();
            ICollection<ViewLink> viewLinks = ViewLinks.Select().ToList();
            foreach (View view in GetViews())
            {
                ViewLink viewLink = viewLinks.SingleOrDefault(_viewLink => _viewLink.ViewId == view.Id);
                Incident incident = viewLink == null ? null : incidents.Single(_incident => _incident.IncidentId == viewLink.IncidentId);
                yield return new Link<View>(view, incident);
            }
        }

        public IEnumerable<Link<Pgm>> GetLinkedPgms()
        {
            ICollection<Incident> incidents = Incidents.Select().ToList();
            ICollection<PgmLink> pgmLinks = PgmLinks.Select().ToList();
            foreach (Pgm pgm in GetPgms())
            {
                PgmLink pgmLink = pgmLinks.SingleOrDefault(_pgmLink => _pgmLink.PgmId == pgm.PgmId);
                Incident incident = pgmLink == null ? null : incidents.Single(_incident => _incident.IncidentId == pgmLink.IncidentId);
                yield return new Link<Pgm>(pgm, incident);
            }
        }

        public IEnumerable<Link<Canvas>> GetLinkedCanvases()
        {
            ICollection<Incident> incidents = Incidents.Select().ToList();
            ICollection<CanvasLink> canvasLinks = CanvasLinks.Select().ToList();
            foreach (Canvas canvas in GetCanvases())
            {
                CanvasLink canvasLink = canvasLinks.SingleOrDefault(_canvasLink => _canvasLink.CanvasId == canvas.CanvasId);
                Incident incident = canvasLink == null ? null : incidents.Single(_incident => _incident.IncidentId == canvasLink.IncidentId);
                yield return new Link<Canvas>(canvas, incident);
            }
        }

        public IEnumerable<View> GetLinkedViews(string incidentId)
        {
            ICollection<int> viewIds = ViewLinks.Select(GetIncidentPredicate(incidentId))
                .Select(viewLink => viewLink.ViewId)
                .ToList();
            return GetViews().Where(view => viewIds.Contains(view.Id));
        }

        public IEnumerable<Pgm> GetLinkedPgms(string incidentId)
        {
            ICollection<int> pgmIds = PgmLinks.Select(GetIncidentPredicate(incidentId))
                .Select(pgmLink => pgmLink.PgmId)
                .ToList();
            return GetPgms().Where(pgm => pgmIds.Contains(pgm.PgmId));
        }

        public IEnumerable<Canvas> GetLinkedCanvases(string incidentId)
        {
            ICollection<int> canvasIds = CanvasLinks.Select(GetIncidentPredicate(incidentId))
                .Select(canvasLink => canvasLink.CanvasId)
                .ToList();
            return GetCanvases().Where(canvas => canvasIds.Contains(canvas.CanvasId));
        }

        public IEnumerable<View> GetUnlinkedViews()
        {
            ICollection<int> viewIds = ViewLinks.Select()
                .Select(viewLink => viewLink.ViewId)
                .ToList();
            return GetViews().Where(view => !viewIds.Contains(view.Id));
        }

        public IEnumerable<Pgm> GetUnlinkedPgms()
        {
            ICollection<int> pgmIds = PgmLinks.Select()
                .Select(pgmLink => pgmLink.PgmId)
                .ToList();
            return GetPgms().Where(pgm => !pgmIds.Contains(pgm.PgmId));
        }

        public IEnumerable<Canvas> GetUnlinkedCanvases()
        {
            ICollection<int> canvasIds = CanvasLinks.Select()
                .Select(canvasLink => canvasLink.CanvasId)
                .ToList();
            return GetCanvases().Where(canvas => !canvasIds.Contains(canvas.CanvasId));
        }

        public bool IsResponderView(View view)
        {
            return view.Name.EqualsIgnoreCase(Responders.View.Name);
        }

        public bool IsResponderLinkedView(View view)
        {
            Responder responder;
            return !IsResponderView(view) && view.Fields.Contains(nameof(responder.ResponderId));
        }
    }
}
