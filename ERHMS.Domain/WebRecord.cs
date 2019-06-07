using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class WebRecord : GuidEntity
    {
        public override string Guid
        {
            get { return WebRecordId; }
            set { WebRecordId = value; }
        }

        public string WebRecordId
        {
            get { return GetProperty<string>(nameof(WebRecordId)); }
            set { SetProperty(nameof(WebRecordId), value); }
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

        public WebRecord(bool @new)
            : base(@new) { }

        public WebRecord()
            : this(false) { }

        public override object Clone()
        {
            WebRecord clone = (WebRecord)base.Clone();
            clone.Responder = (Responder)Responder.Clone();
            return clone;
        }
    }
}
