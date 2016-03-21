using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using Project = ERHMS.EpiInfo.Project;
using Template = ERHMS.EpiInfo.Template;

namespace ERHMS.DataAccess
{
    public class DataContext
    {
        public IDataDriver Driver { get; private set; }

        public Project Project
        {
            get { return Driver.Project; }
        }

        public CodeRepository Prefixes { get; private set; }
        public CodeRepository Suffixes { get; private set; }
        public CodeRepository Genders { get; private set; }
        public CodeRepository States { get; private set; }
        public ResponderRepository Responders { get; private set; }
        public IncidentRepository Incidents { get; private set; }
        public LocationRepository Locations { get; private set; }

        public DataContext(IDataDriver driver)
        {
            Driver = driver;
            Prefixes = new CodeRepository(driver, "codeprefix1", "prefix", false);
            Suffixes = new CodeRepository(driver, "codesuffix1", "suffix", false);
            Genders = new CodeRepository(driver, "codegender1", "gender", false);
            States = new CodeRepository(driver, "codestate1", "state", true);
            Responders = new ResponderRepository(driver);
            Incidents = new IncidentRepository(driver);
            Locations = new LocationRepository(driver);
        }

        public IEnumerable<View> GetViews()
        {
            return Project.GetViews();
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

        public IEnumerable<Canvas> GetCanvases()
        {
            return Canvas.GetByProject(Project);
        }
    }
}
