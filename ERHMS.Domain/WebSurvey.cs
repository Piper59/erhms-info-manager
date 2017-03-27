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
            get { return GetProperty<string>(nameof(WebSurveyId)); }
            set { SetProperty(nameof(WebSurveyId), value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>(nameof(ViewId)); }
            set { SetProperty(nameof(ViewId), value); }
        }

        public string PublishKey
        {
            get { return GetProperty<string>(nameof(PublishKey)); }
            set { SetProperty(nameof(PublishKey), value); }
        }

        public WebSurvey()
        {
            AddSynonym(nameof(WebSurveyId), nameof(Guid));
        }
    }
}
