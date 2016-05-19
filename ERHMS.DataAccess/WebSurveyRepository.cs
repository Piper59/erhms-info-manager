using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;

namespace ERHMS.DataAccess
{
    public class WebSurveyRepository : TableEntityRepository<WebSurvey>
    {
        public WebSurveyRepository(IDataDriver driver)
            : base(driver, "ERHMS_WebSurveys")
        { }

        public void DeleteByViewId(int viewId)
        {
            DataParameterCollection parameters = new DataParameterCollection(Driver);
            parameters.AddByValue(viewId);
            Delete(parameters.ToPredicate("ViewId = {0}"));
        }
    }
}
