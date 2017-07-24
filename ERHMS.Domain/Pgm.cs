using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Pgm : Entity
    {
        public int PgmId
        {
            get { return GetProperty<int>(nameof(PgmId)); }
            set { SetProperty(nameof(PgmId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public string Content
        {
            get { return GetProperty<string>(nameof(Content)); }
            set { SetProperty(nameof(Content), value); }
        }

        public string Comment
        {
            get { return GetProperty<string>(nameof(Comment)); }
            set { SetProperty(nameof(Comment), value); }
        }

        public string Author
        {
            get { return GetProperty<string>(nameof(Author)); }
            set { SetProperty(nameof(Author), value); }
        }

        public string PgmLinkId
        {
            get { return GetProperty<string>(nameof(PgmLinkId)); }
            set { SetProperty(nameof(PgmLinkId), value); }
        }

        private PgmLink pgmLink;
        public PgmLink PgmLink
        {
            get { return pgmLink; }
            set { SetProperty(nameof(PgmLink), ref pgmLink, value); }
        }

        public Incident Incident
        {
            get { return PgmLink?.Incident; }
        }

        public EpiInfo.Pgm ToEpiInfo()
        {
            return new EpiInfo.Pgm
            {
                PgmId = PgmId,
                Name = Name,
                Content = Content,
                Comment = Comment,
                Author = Author
            };
        }
    }
}
