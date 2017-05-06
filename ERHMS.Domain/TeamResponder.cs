using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class TeamResponder : TableEntity
    {
        public override string Guid
        {
            get { return TeamResponderId; }
            set { TeamResponderId = value; }
        }

        public string TeamResponderId
        {
            get { return GetProperty<string>(nameof(TeamResponderId)); }
            set { SetProperty(nameof(TeamResponderId), value); }
        }

        public string TeamId
        {
            get { return GetProperty<string>(nameof(TeamId)); }
            set { SetProperty(nameof(TeamId), value); }
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

        public TeamResponder()
        {
            AddSynonym(nameof(TeamResponderId), nameof(Guid));
        }
    }
}
