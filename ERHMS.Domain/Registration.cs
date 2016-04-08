﻿using ERHMS.EpiInfo.Domain;

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

        public string ResponderId
        {
            get { return GetProperty<string>("ResponderId"); }
            set { SetProperty("ResponderId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
