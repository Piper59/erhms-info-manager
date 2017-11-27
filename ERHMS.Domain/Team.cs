namespace ERHMS.Domain
{
    public class Team : IncidentEntity
    {
        protected override string Guid
        {
            get { return TeamId; }
            set { TeamId = value; }
        }

        public string TeamId
        {
            get { return GetProperty<string>(nameof(TeamId)); }
            set { SetProperty(nameof(TeamId), value); }
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

        public Team(bool @new)
            : base(@new) { }

        public Team()
            : this(false) { }
    }
}
