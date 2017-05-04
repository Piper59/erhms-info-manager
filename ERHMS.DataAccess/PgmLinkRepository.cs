using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class PgmLinkRepository : LinkRepository<PgmLink, Pgm>
    {
        public PgmLinkRepository(Project project, IDataDriver driver, IncidentRepository incidents)
            : base(project, driver, incidents, "ERHMS_PgmLinks") { }

        public override IEnumerable<Pgm> SelectItems()
        {
            return Project.GetPgms();
        }

        public void DeleteByPgmId(int pgmId)
        {
            Delete("[PgmId] = {@}", pgmId);
        }
    }
}
