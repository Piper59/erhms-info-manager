using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class ResponderViewEntity : ViewEntity
    {
        private View view;
        public View View
        {
            get { return view; }
            set { SetProperty(nameof(View), ref view, value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>("ResponderID"); }
            set { SetProperty("ResponderID", value); }
        }
    }
}
