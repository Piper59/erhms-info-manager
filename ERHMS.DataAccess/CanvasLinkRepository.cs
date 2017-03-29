using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : LinkRepository<CanvasLink, Canvas>
    {
        public CanvasLinkRepository(IDataDriver driver, DataContext dataContext)
            : base(driver, "ERHMS_CanvasLinks", dataContext) { }

        public override IEnumerable<Canvas> SelectItems()
        {
            return DataContext.GetCanvases();
        }

        public void DeleteByCanvasId(int canvasId)
        {
            Delete("[CanvasId] = {@}", canvasId);
        }
    }
}
