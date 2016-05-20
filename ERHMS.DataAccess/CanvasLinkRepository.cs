using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : TableEntityRepository<CanvasLink>
    {
        public CanvasLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_CanvasLinks") { }

        public IEnumerable<CanvasLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }

        public void DeleteByCanvasId(int canvasId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(canvasId);
            Delete(parameters.ToPredicate("CanvasId = {0}"));
        }
    }
}
