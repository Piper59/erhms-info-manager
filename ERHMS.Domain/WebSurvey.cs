using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class WebSurvey : Entity
    {
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

        public WebSurvey()
        {
            WebSurveyId = Guid.NewGuid().ToString();
        }
    }
}
