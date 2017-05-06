using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class JobTeam : TableEntity
    {
        public override string Guid
        {
            get { return JobTeamId; }
            set { JobTeamId = value; }
        }

        public string JobTeamId
        {
            get { return GetProperty<string>(nameof(JobTeamId)); }
            set { SetProperty(nameof(JobTeamId), value); }
        }

        public string JobId
        {
            get { return GetProperty<string>(nameof(JobId)); }
            set { SetProperty(nameof(JobId), value); }
        }

        public string TeamId
        {
            get { return GetProperty<string>(nameof(TeamId)); }
            set { SetProperty(nameof(TeamId), value); }
        }

        public JobTeam()
        {
            AddSynonym(nameof(JobTeamId), nameof(Guid));
        }
    }
}
