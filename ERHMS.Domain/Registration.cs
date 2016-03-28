using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Registration : TableEntity
    {
        public override string Guid
        {
            get { return RegistrationId; }
            set { RegistrationId = value; }
        }

        public string RegistrationId
        {
            get { return GetProperty<string>("RegistrationId"); }
            set { SetProperty("RegistrationId", value); }
        }
    }
}
