using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;
using Template = ERHMS.EpiInfo.Template;

namespace ERHMS.DataAccess
{
    public class DataContext
    {
        public Project Project { get; private set; }
        public IDataDriver Driver { get; private set; }
        public CodeRepository Prefixes { get; private set; }
        public CodeRepository Suffixes { get; private set; }
        public CodeRepository Genders { get; private set; }
        public CodeRepository States { get; private set; }
        public ResponderRepository Responders { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public LocationRepository Locations { get; private set; }
        public RegistrationRepository Registrations { get; private set; }
        public AssignmentRepository Assignments { get; private set; }
        public ViewLinkRepository ViewLinks { get; private set; }
        public PgmLinkRepository PgmLinks { get; private set; }
        public CanvasLinkRepository CanvasLinks { get; private set; }

        public DataContext(Project project)
        {
            Project = project;
            Driver = DataDriverFactory.CreateDataDriver(project);
            Prefixes = new CodeRepository(Driver, "codeprefix1", "prefix", false);
            Suffixes = new CodeRepository(Driver, "codesuffix1", "suffix", false);
            Genders = new CodeRepository(Driver, "codegender1", "gender", false);
            States = new CodeRepository(Driver, "codestate1", "state", true);
            Responders = new ResponderRepository(Driver, project);
            Incidents = new IncidentRepository(Driver);
            Locations = new LocationRepository(Driver);
            Registrations = new RegistrationRepository(Driver);
            Assignments = new AssignmentRepository(Driver);
            ViewLinks = new ViewLinkRepository(Driver);
            PgmLinks = new PgmLinkRepository(Driver);
            CanvasLinks = new CanvasLinkRepository(Driver);
        }

        private DataPredicate GetLinkPredicate(string incidentId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(incidentId);
            string sql = parameters.Format("IncidentId = {0}");
            return new DataPredicate(sql, parameters);
        }

        public IEnumerable<View> GetViews()
        {
            return Project.GetViews();
        }

        public IEnumerable<View> GetLinkedViews(string incidentId)
        {
            ICollection<int> viewIds = ViewLinks.Select(GetLinkPredicate(incidentId))
                .Select(viewLink => viewLink.ViewId)
                .ToList();
            return GetViews().Where(view => viewIds.Contains(view.Id));
        }

        public IEnumerable<Template> GetTemplates(TemplateLevel? level = null)
        {
            if (level.HasValue)
            {
                return Template.GetByLevel(level.Value);
            }
            else
            {
                return Template.GetAll();
            }
        }

        public IEnumerable<Pgm> GetPgms()
        {
            return Pgm.GetByProject(Project);
        }

        public IEnumerable<Pgm> GetLinkedPgms(string incidentId)
        {
            ICollection<PgmLink> pgmLinks = PgmLinks.Select(GetLinkPredicate(incidentId)).ToList();
            ICollection<int> pgmIds = pgmLinks.Where(pgmLink => pgmLink.PgmId.HasValue)
                .Select(pgmLink => pgmLink.PgmId.Value)
                .ToList();
            ICollection<string> paths = pgmLinks.Where(pgmLink => pgmLink.Path != null)
                .Select(pgmLink => pgmLink.Path.ToLower())
                .ToList();
            return GetPgms().Where(pgm => pgm.Id.HasValue && pgmIds.Contains(pgm.Id.Value) || paths.Contains(pgm.File.FullName.ToLower()));
        }

        public IEnumerable<Canvas> GetCanvases()
        {
            return Canvas.GetByProject(Project);
        }

        public IEnumerable<Canvas> GetLinkedCanvases(string incidentId)
        {
            ICollection<string> paths = CanvasLinks.Select(GetLinkPredicate(incidentId))
                .Select(canvasLink => canvasLink.Path.ToLower())
                .ToList();
            return GetCanvases().Where(canvas => paths.Contains(canvas.File.FullName.ToLower()));
        }
    }
}
