using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Team : TableEntity
    {
        public override string Guid
        {
            get { return TeamId; }
            set { TeamId = value; }
        }

        public string TeamId
        {
            get { return GetProperty<string>(nameof(TeamId)); }
            set { SetProperty(nameof(TeamId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public string Description
        {
            get { return GetProperty<string>(nameof(Description)); }
            set { SetProperty(nameof(Description), value); }
        }

        public Team()
        {
            AddSynonym(nameof(TeamId), nameof(Guid));
        }
    }
}
