using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Role : Entity
    {
        public string RoleId
        {
            get { return GetProperty<string>(nameof(RoleId)); }
            set { SetProperty(nameof(RoleId), value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public Role()
        {
            RoleId = Guid.NewGuid().ToString();
        }
    }
}
