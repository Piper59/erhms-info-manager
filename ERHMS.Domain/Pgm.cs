namespace ERHMS.Domain
{
    public class Pgm : EpiInfoEntity<PgmLink>
    {
        protected override object Id
        {
            get { return PgmId; }
        }

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

        private PgmLink link;
        public override PgmLink Link
        {
            get { return link; }
            set { SetProperty(nameof(Link), ref link, value); }
        }

        public Pgm(bool @new)
            : base(@new) { }

        public Pgm()
            : this(false) { }

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
