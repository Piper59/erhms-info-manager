namespace ERHMS.Domain
{
    public class PgmLink : IncidentEntity
    {
        protected override string Guid
        {
            get { return PgmLinkId; }
            set { PgmLinkId = value; }
        }

        public string PgmLinkId
        {
            get { return GetProperty<string>(nameof(PgmLinkId)); }
            set { SetProperty(nameof(PgmLinkId), value); }
        }

        public int PgmId
        {
            get { return GetProperty<int>(nameof(PgmId)); }
            set { SetProperty(nameof(PgmId), value); }
        }

        public PgmLink(bool @new)
            : base(@new) { }

        public PgmLink()
            : this(false) { }
    }
}
