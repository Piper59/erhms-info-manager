using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class AssignmentRepository : TableEntityRepository<Assignment>
    {
        public AssignmentRepository(IDataDriver driver)
            : base(driver, "ERHMS_Assignments") { }

        public void DeleteByViewId(int viewId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(viewId);
            Delete(parameters.ToPredicate("ViewId = {0}"));
        }
    }
}
