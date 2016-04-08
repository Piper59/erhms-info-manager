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
            get { return GetProperty<string>("AssignmentId"); }
            set { SetProperty("AssignmentId", value); }
        }

        public int ViewId
        {
            get { return GetProperty<int>("ViewId"); }
            set { SetProperty("ViewId", value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>("ResponderId"); }
            set { SetProperty("ResponderId", value); }
        }
    }
}
