using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class JobResponder : TableEntity
    {
        public override string Guid
        {
            get { return JobResponderId; }
            set { JobResponderId = value; }
        }

        public string JobResponderId
        {
            get { return GetProperty<string>(nameof(JobResponderId)); }
            set { SetProperty(nameof(JobResponderId), value); }
        }

        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
        }

        public string ResponderId
        {
            get { return GetProperty<string>(nameof(ResponderId)); }
            set { SetProperty(nameof(ResponderId), value); }
        }

        public string RoleId
        {
            get { return GetProperty<string>(nameof(RoleId)); }
            set { SetProperty(nameof(RoleId), value); }
        }

        public JobResponder()
        {
            AddSynonym(nameof(JobResponderId), nameof(Guid));
        }
    }
}
