using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class ViewLinkRepository : LinkRepository<ViewLink, View>
    {
        public ViewLinkRepository(IDataDriver driver, DataContext dataContext)
            : base(driver, "ERHMS_ViewLinks", dataContext) { }

        protected override IEnumerable<View> GetItems()
        {
            return DataContext.GetViews();
        }

        public void DeleteByViewId(int viewId)
        {
            Delete("[ViewId] = {@}", viewId);
        }
    }
}
