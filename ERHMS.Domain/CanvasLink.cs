using System;

namespace ERHMS.Domain
{
    public class CanvasLink : Link
    {
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
            CanvasLinkId = Guid.NewGuid().ToString();
        }
    }
}
