using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Role : TableEntity
    {
        public override string Guid
        {
            get { return RoleId; }
            set { RoleId = value; }
        }

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
            AddSynonym(nameof(RoleId), nameof(Guid));
        }
    }
}
