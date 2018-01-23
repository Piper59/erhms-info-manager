namespace ERHMS.Domain
{
    public class Canvas : EpiInfoEntity<CanvasLink>
    {
        public override object Id
        {
            get { return CanvasId; }
        }

        public int CanvasId
        {
            get { return GetProperty<int>(nameof(CanvasId)); }
            set { SetProperty(nameof(CanvasId), value); }
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

        public string CanvasLinkId
        {
            get { return GetProperty<string>(nameof(CanvasLinkId)); }
            set { SetProperty(nameof(CanvasLinkId), value); }
        }

        private CanvasLink link;
        public override CanvasLink Link
        {
            get { return link; }
            set { SetProperty(nameof(Link), ref link, value); }
        }

        public Canvas(bool @new)
            : base(@new) { }

        public Canvas()
            : this(false) { }

        public EpiInfo.Canvas ToEpiInfo()
        {
            return new EpiInfo.Canvas
            {
                CanvasId = CanvasId,
                Name = Name,
                Content = Content
            };
        }
    }
}
