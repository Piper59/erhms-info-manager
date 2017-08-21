using System;

namespace ERHMS.Domain
{
    public class IncidentRole : Link
    {
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

        public bool IsInUse
        {
            get { return GetProperty<bool>(nameof(IsInUse)); }
            set { SetProperty(nameof(IsInUse), value); }
        }

        public IncidentRole()
        {
            IncidentRoleId = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
