﻿using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.DataAccess
{
    // TODO: Add upgrade hooks
    public class DataContext
    {
        public static DataContext Create(Project project)
        {
            Log.Logger.DebugFormat("Creating data context: {0}", project.FilePath);
            string templatePath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.DataAccess.Resources.Project.xml", templatePath);
            TemplateInfo templateInfo = TemplateInfo.Get(templatePath);
            Wrapper wrapper = MakeView.InstantiateProjectTemplate.Create(project.FilePath, templatePath);
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
            Log.Logger.DebugFormat("Opening data context: {0}", project.FilePath);
            Project = project;
            Driver = DataDriverFactory.CreateDataDriver(project);
            Prefixes = new CodeRepository(Driver, "codeprefix1", "prefix", false);
            Suffixes = new CodeRepository(Driver, "codesuffix1", "suffix", false);
            Genders = new CodeRepository(Driver, "codegender1", "gender", false);
            States = new CodeRepository(Driver, "codestate1", "state", true);
            Rosters = new RosterRepository(Driver);
            Responders = new ResponderRepository(Driver, project, Rosters);
            Incidents = new IncidentRepository(Driver);
            IncidentNotes = new IncidentNoteRepository(Driver);
            Locations = new LocationRepository(Driver);
            Assignments = new AssignmentRepository(Driver);
            ViewLinks = new ViewLinkRepository(project, Driver, Incidents);
            PgmLinks = new PgmLinkRepository(project, Driver, Incidents);
            CanvasLinks = new CanvasLinkRepository(project, Driver, Incidents);
            WebSurveys = new WebSurveyRepository(Driver);
        }

        public TemplateInfo CreateNewViewTemplate()
        {
            string path = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.DataAccess.Resources.View.xml", path);
            return TemplateInfo.Get(path);
        }

        public bool IsResponderView(View view)
        {
            return view.Name.EqualsIgnoreCase(Responders.View.Name);
        }

        public bool IsResponderLinkedView(View view)
        {
            return !IsResponderView(view) && view.Fields.Contains(nameof(Responder.ResponderId));
        }
    }
}
