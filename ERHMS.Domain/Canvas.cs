using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Canvas : Entity
    {
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

        private CanvasLink canvasLink;
        public CanvasLink CanvasLink
        {
            get { return canvasLink; }
            set { SetProperty(nameof(CanvasLink), ref canvasLink, value); }
        }

        public Incident Incident
        {
            get { return CanvasLink?.Incident; }
        }

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
