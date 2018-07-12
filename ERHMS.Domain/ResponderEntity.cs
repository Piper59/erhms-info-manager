using ERHMS.EpiInfo.Domain;
using System.Collections.Generic;

namespace ERHMS.Domain
{
    public class ResponderEntity : ViewEntity
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

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }

        public ResponderEntity()
            : base() { }

        public ResponderEntity(ViewEntity @base)
            : this()
        {
            foreach (KeyValuePair<string, object> property in @base.GetProperties())
            {
                SetProperty(property.Key, property.Value);
            }
        }
    }
}
