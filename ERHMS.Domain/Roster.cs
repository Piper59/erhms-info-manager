namespace ERHMS.Domain
{
    public class Roster : IncidentEntity
    {
        public override string Guid
        {
            get { return RosterId; }
            set { RosterId = value; }
        }

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

        public Roster(bool @new)
            : base(@new) { }

        public Roster()
            : this(false) { }

        public override object Clone()
        {
            Roster clone = (Roster)base.Clone();
            clone.Responder = (Responder)Responder.Clone();
            return clone;
        }
    }
}
