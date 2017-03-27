using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : LinkRepository<PgmLink, Pgm>
    {
        public PgmLinkRepository(IDataDriver driver, DataContext dataContext)
            : base(driver, "ERHMS_PgmLinks", dataContext) { }

        protected override IEnumerable<Pgm> GetItems()
        {
            return DataContext.GetPgms();
        }

        public void DeleteByPgmId(int pgmId)
        {
            Delete("[PgmId] = {@}", pgmId);
        }
    }
}
