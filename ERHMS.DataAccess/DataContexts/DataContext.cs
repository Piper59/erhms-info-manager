using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using System.Collections.Generic;
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
            IncidentRoleRepository.Configure();
            JobRepository.Configure();
            JobLocationRepository.Configure();
            JobNoteRepository.Configure();
            JobResponderRepository.Configure();
            JobTeamRepository.Configure();
            LocationRepository.Configure();
            PgmRepository.Configure();
            PgmLinkRepository.Configure();
            ResponderRepository.Configure();
            RoleRepository.Configure();
            RosterRepository.Configure();
            TeamRepository.Configure();
            TeamResponderRepository.Configure();
            UniquePairRepository.Configure();
            ViewRepository.Configure();
            ViewLinkRepository.Configure();
            WebSurveyRepository.Configure();
        }

        private static Script GetScript(string name)
        {
            string resourceName = string.Format("ERHMS.DataAccess.Scripts.{0}.sql", name);
            return new Script(Assembly.GetExecutingAssembly().GetManifestResourceText(resourceName));
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
            foreach (Epi.View view in project.Views)
            {
                project.CollectedData.CreateDataTableForView(view, 1);
            }
            DataContext context = new DataContext(project);
            context.Database.Invoke((connection, transaction) =>
            {
                connection.Execute(GetScript("Base"), transaction);
                context.Upgrade();
            });
            return context;
        }

        public static TemplateInfo GetNewViewTemplate()
        {
            string path = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.DataAccess.Resources.View.xml", path);
            return TemplateInfo.Get(path);
        }

        public AssignmentRepository Assignments { get; private set; }
        public CanvasRepository Canvases { get; private set; }
        public CanvasLinkRepository CanvasLinks { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public IncidentNoteRepository IncidentNotes { get; private set; }
        public IncidentRoleRepository IncidentRoles { get; private set; }
        public JobRepository Jobs { get; private set; }
        public JobLocationRepository JobLocations { get; private set; }
        public JobNoteRepository JobNotes { get; private set; }
        public JobResponderRepository JobResponders { get; private set; }
        public JobTeamRepository JobTeams { get; private set; }
        public LocationRepository Locations { get; private set; }
        public PgmRepository Pgms { get; private set; }
        public PgmLinkRepository PgmLinks { get; private set; }
        public ResponderRepository Responders { get; private set; }
        public RoleRepository Roles { get; private set; }
        public RosterRepository Rosters { get; private set; }
        public TeamRepository Teams { get; private set; }
        public TeamResponderRepository TeamResponders { get; private set; }
        public UniquePairRepository UniquePairs { get; private set; }
        public ViewRepository Views { get; private set; }
        public ViewLinkRepository ViewLinks { get; private set; }
        public WebSurveyRepository WebSurveys { get; private set; }

        public IEnumerable<string> Prefixes
        {
            get { return GetCodes("codeprefix1", "prefix"); }
        }

        public IEnumerable<string> Suffixes
        {
            get { return GetCodes("codesuffix1", "suffix"); }
        }

        public IEnumerable<string> Genders
        {
            get { return GetCodes("codegender1", "gender"); }
        }

        public IEnumerable<string> States
        {
            get { return GetCodes("codestate1", "state"); }
        }

        public DataContext(Project project)
            : base(project)
        {
            Assignments = new AssignmentRepository(this);
            Canvases = new CanvasRepository(this);
            CanvasLinks = new CanvasLinkRepository(this);
            Incidents = new IncidentRepository(this);
            IncidentNotes = new IncidentNoteRepository(this);
            IncidentRoles = new IncidentRoleRepository(this);
            Jobs = new JobRepository(this);
            JobLocations = new JobLocationRepository(this);
            JobNotes = new JobNoteRepository(this);
            JobResponders = new JobResponderRepository(this);
            JobTeams = new JobTeamRepository(this);
            Locations = new LocationRepository(this);
            Pgms = new PgmRepository(this);
            PgmLinks = new PgmLinkRepository(this);
            Responders = new ResponderRepository(this);
            Roles = new RoleRepository(this);
            Rosters = new RosterRepository(this);
            Teams = new TeamRepository(this);
            TeamResponders = new TeamResponderRepository(this);
            UniquePairs = new UniquePairRepository(this);
            Views = new ViewRepository(this);
            ViewLinks = new ViewLinkRepository(this);
            WebSurveys = new WebSurveyRepository(this);
        }

        private IEnumerable<string> GetCodes(string tableName, string columnName)
        {
            return Project.GetCodes(tableName, columnName, false).Prepend("");
        }

        public bool NeedsUpgrade()
        {
            if (!Database.TableExists("ERHMS_Jobs"))
            {
                return true;
            }
            if (!Database.TableExists("ERHMS_UniquePairs"))
            {
                return true;
            }
            return false;
        }

        public void Upgrade()
        {
            Log.Logger.DebugFormat("Upgrading data context: {0}", Project.FilePath);
            Database.Transact((connection, transaction) =>
            {
                if (!Database.TableExists("ERHMS_Jobs"))
                {
                    connection.Execute(GetScript("JobTicketing"), transaction);
                    foreach (Incident incident in Incidents.Select())
                    {
                        IncidentRoles.InsertAll(incident.IncidentId);
                    }
                }
                if (!Database.TableExists("ERHMS_UniquePairs"))
                {
                    connection.Execute(GetScript("Deduplication"), transaction);
                }
            });
            Project.Version = Assembly.GetExecutingAssembly().GetName().Version;
            Project.Save();
        }
    }
}
