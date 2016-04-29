using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class CanvasLinkRepository : TableEntityRepository<CanvasLink>
    {
        public CanvasLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_CanvasLinks")
        { }

        public IEnumerable<CanvasLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }

        public CanvasLink SelectByCanvasId(int canvasId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(canvasId);
            string sql = parameters.Format("CanvasId = {0}");
            return Select(new DataPredicate(sql, parameters)).SingleOrDefault();
        }
    }
}
