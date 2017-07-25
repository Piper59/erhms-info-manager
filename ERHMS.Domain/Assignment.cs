using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Assignment : Entity
    {
        public string AssignmentId
        {
            get { return GetProperty<string>(nameof(AssignmentId)); }
            set { SetProperty(nameof(AssignmentId), value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>(nameof(ViewId)); }
            set { SetProperty(nameof(ViewId), value); }
        }

        private View view;
        public View View
        {
            get { return view; }
            set { SetProperty(nameof(View), ref view, value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { SetProperty(nameof(Responder), ref responder, value); }
        }

        public Assignment()
        {
            AssignmentId = Guid.NewGuid().ToString();
        }

        public override object Clone()
        {
            Assignment clone = (Assignment)base.Clone();
            clone.View = (View)View.Clone();
            clone.Responder = (Responder)Responder.Clone();
            return clone;
        }
    }
}
