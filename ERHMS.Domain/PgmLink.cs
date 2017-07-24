using System;

namespace ERHMS.Domain
{
    public class PgmLink : Link
    {
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
            PgmLinkId = Guid.NewGuid().ToString();
        }
    }
}
