using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class WebSurvey : GuidEntity
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

        private View view;
        public View View
        {
            get { return view; }
            set { SetProperty(nameof(View), ref view, value); }
        }

        public string PublishKey
        {
            get { return GetProperty<string>(nameof(PublishKey)); }
            set { SetProperty(nameof(PublishKey), value); }
        }

        public WebSurvey(bool @new)
            : base(@new) { }

        public WebSurvey()
            : this(false) { }

        public override object Clone()
        {
            WebSurvey clone = (WebSurvey)base.Clone();
            clone.View = (View)View.Clone();
            return clone;
        }
    }
}
