using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : LinkRepository<ViewLink, View>
    {
        public ViewLinkRepository(Project project, IDataDriver driver, IncidentRepository incidents)
            : base(project, driver, incidents, "ERHMS_ViewLinks") { }

        public override IEnumerable<View> SelectItems()
        {
            return Project.GetViews();
        }

        public void DeleteByViewId(int viewId)
        {
            Delete("[ViewId] = {@}", viewId);
        }
    }
}
