namespace ERHMS.Domain
{
    public class IncidentRole : IncidentEntity
    {
        public override string Guid
        {
            get { return IncidentRoleId; }
            set { IncidentRoleId = value; }
        }

        public string IncidentRoleId
        {
            get { return GetProperty<string>(nameof(IncidentRoleId)); }
            set { SetProperty(nameof(IncidentRoleId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public bool InUse
        {
            get { return GetProperty<bool>(nameof(InUse)); }
            set { SetProperty(nameof(InUse), value); }
        }

        public IncidentRole(bool @new)
            : base(@new) { }

        public IncidentRole()
            : this(false) { }

        public override string ToString()
        {
            return Name;
        }
    }
}
