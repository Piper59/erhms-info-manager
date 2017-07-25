using System;

namespace ERHMS.Domain
{
    public class Roster : Link
    {
        public string RosterId
        {
            get { return GetProperty<string>(nameof(RosterId)); }
            set { SetProperty(nameof(RosterId), value); }
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

        public Roster()
        {
            RosterId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            Roster clone = (Roster)base.Clone();
            clone.Responder = (Responder)Responder.Clone();
            return clone;
        }
    }
}
