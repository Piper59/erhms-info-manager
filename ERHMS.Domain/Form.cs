using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Form : TableEntity
    {
        public override string Guid
        {
            get { return FormId; }
            set { FormId = value; }
        }

        public string FormId
        {
            get { return GetProperty<string>("FormId"); }
            set { SetProperty("FormId", value); }
        }
        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
