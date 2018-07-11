using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class WebLink : GuidEntity
    {
        public override string Guid
        {
            get { return WebLinkId; }
            set { WebLinkId = value; }
        }

        public string WebLinkId
        {
            get { return GetProperty<string>(nameof(WebLinkId)); }
            set { SetProperty(nameof(WebLinkId), value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }

        public string WebSurveyId
        {
            get { return GetProperty<string>(nameof(WebSurveyId)); }
            set { SetProperty(nameof(WebSurveyId), value); }
        }

        public string GlobalRecordId
        {
            get { return GetProperty<string>(nameof(GlobalRecordId)); }
            set { SetProperty(nameof(GlobalRecordId), value); }
        }

        public WebLink(bool @new)
            : base(@new) { }

        public WebLink()
            : this(false) { }

        public override object Clone()
        {
            WebLink clone = (WebLink)base.Clone();
            clone.Responder = (Responder)Responder.Clone();
            return clone;
        }
    }
}
