using System;

namespace ERHMS.Domain
{
    public class ViewLink : Link
    {
        public string ViewLinkId
        {
            get { return GetProperty<string>(nameof(ViewLinkId)); }
            set { SetProperty(nameof(ViewLinkId), value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>(nameof(ViewId)); }
            set { SetProperty(nameof(ViewId), value); }
        }

        public ViewLink()
        {
            ViewLinkId = Guid.NewGuid().ToString();
        }
    }
}
