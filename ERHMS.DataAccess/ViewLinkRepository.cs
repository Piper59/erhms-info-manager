using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : TableEntityRepository<ViewLink>
    {
        public ViewLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_ViewLinks")
        { }

        public IEnumerable<ViewLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }

        public ViewLink SelectByViewId(int viewId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(viewId);
            string sql = parameters.Format("ViewId = {0}");
            return Select(new DataPredicate(sql, parameters)).SingleOrDefault();
        }
    }
}
