using ERHMS.EpiInfo;

namespace ERHMS.Domain
{
    public class CanvasLink : Link<Canvas>
    {
        public override string Guid
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

        public CanvasLink()
        {
            AddSynonym(nameof(CanvasLinkId), nameof(Guid));
        }

        public override bool IsEqual(Canvas item)
        {
            return CanvasId == item.CanvasId;
        }
    }
}
