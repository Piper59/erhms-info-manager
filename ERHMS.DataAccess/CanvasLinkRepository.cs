using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : LinkRepository<CanvasLink, Canvas>
    {
        public CanvasLinkRepository(Project project, IDataDriver driver, IncidentRepository incidents)
            : base(project, driver, incidents, "ERHMS_CanvasLinks") { }

        public override IEnumerable<Canvas> SelectItems()
        {
            return Project.GetCanvases();
        }

        public void DeleteByCanvasId(int canvasId)
        {
            Delete("[CanvasId] = {@}", canvasId);
        }
    }
}
