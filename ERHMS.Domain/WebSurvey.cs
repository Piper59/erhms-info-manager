using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class WebSurvey : TableEntity
    {
        public override string Guid
        {
            get { return WebSurveyId; }
            set { WebSurveyId = value; }
        }

        public string WebSurveyId
        {
            get { return GetProperty<string>("WebSurveyId"); }
            set { SetProperty("WebSurveyId", value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>("ViewId"); }
            set { SetProperty("ViewId", value); }
        }

        public string PublishKey
        {
            get { return GetProperty<string>("PublishKey"); }
            set { SetProperty("PublishKey", value); }
        }

        public WebSurvey()
        {
            LinkProperties("WebSurveyId", "Guid");
        }
    }
}
