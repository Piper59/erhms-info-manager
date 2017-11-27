namespace ERHMS.Domain
{
    public class CanvasLink : IncidentEntity
    {
        protected override string Guid
        {
            get { return CanvasLinkId; }
            set { CanvasLinkId = value; }
        }

        public string CanvasLinkId
        {
            get { return GetProperty<string>(nameof(CanvasLinkId)); }
            set { SetProperty(nameof(CanvasLinkId), value); }
        }

        public int CanvasId
        {
            get { return GetProperty<int>(nameof(CanvasId)); }
            set { SetProperty(nameof(CanvasId), value); }
        }

        public CanvasLink(bool @new)
            : base(@new) { }

        public CanvasLink()
            : this(false) { }
    }
}
