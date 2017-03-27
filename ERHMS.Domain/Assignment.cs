using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Assignment : TableEntity
    {
        public override string Guid
        {
            get { return AssignmentId; }
            set { AssignmentId = value; }
        }

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

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        public Assignment()
        {
            AddSynonym(nameof(AssignmentId), nameof(Guid));
        }
    }
}
