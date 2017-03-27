using ERHMS.EpiInfo;

namespace ERHMS.Domain
{
    public class PgmLink : Link<Pgm>
    {
        public override string Guid
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

        public PgmLink()
        {
            AddSynonym(nameof(PgmLinkId), nameof(Guid));
        }

        public override bool IsEqual(Pgm item)
        {
            return PgmId == item.PgmId;
        }
    }
}
