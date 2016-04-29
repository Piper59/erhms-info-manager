using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : TableEntityRepository<PgmLink>
    {
        public PgmLinkRepository(IDataDriver driver)
            : base(driver, "ERHMS_PgmLinks")
        { }

        public IEnumerable<PgmLink> SelectByIncident(string incidentId)
        {
            return Select(DataContext.GetIncidentPredicate(Driver, incidentId));
        }

        public PgmLink SelectByPgmId(int pgmId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(pgmId);
            string sql = parameters.Format("PgmId = {0}");
            return Select(new DataPredicate(sql, parameters)).SingleOrDefault();
        }
    }
}
