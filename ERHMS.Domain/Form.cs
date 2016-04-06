using ERHMS.EpiInfo.Domain;

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

        public int ViewId
        {
            get { return GetProperty<int>("ViewId"); }
            set { SetProperty("ViewId", value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }
    }
}
