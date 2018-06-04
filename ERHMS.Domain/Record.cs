using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Record : ViewEntity
    {
        private View view;
        public View View
        {
            get { return view; }
            set { SetProperty(nameof(View), ref view, value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(FieldNames.ResponderId); }
            set { SetProperty(FieldNames.ResponderId, value); }
        }
    }
}
