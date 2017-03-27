using Epi;

namespace ERHMS.Domain
{
    public class ViewLink : Link<View>
    {
        public override string Guid
        {
            get { return ViewLinkId; }
            set { ViewLinkId = value; }
        }

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
            AddSynonym(nameof(ViewLinkId), nameof(Guid));
        }

        public override bool IsEqual(View item)
        {
            return ViewId == item.Id;
        }
    }
}
