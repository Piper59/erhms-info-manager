using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;

namespace ERHMS.Test
{
    public class Surveillance : ViewEntity
    {
        public string CaseId
        {
            get { return GetProperty<string>("CaseID"); }
            set { SetProperty("CaseID", value); }
        }

        public string LastName
        {
            get { return GetProperty<string>(nameof(LastName)); }
            set { SetProperty(nameof(LastName), value); }
        }

        public string FirstName
        {
            get { return GetProperty<string>(nameof(FirstName)); }
            set { SetProperty(nameof(FirstName), value); }
        }

        public byte? Hospitalized
        {
            get { return GetProperty<byte?>(nameof(Hospitalized)); }
            set { SetProperty(nameof(Hospitalized), value); }
        }

        public DateTime? EnteredOn
        {
            get { return GetProperty<DateTime?>("Entered"); }
            set { SetProperty("Entered", value?.RemoveMilliseconds()); }
        }

        public Surveillance()
        {
            AddSynonym("CaseID", nameof(CaseId));
            AddSynonym("Entered", nameof(EnteredOn));
        }
    }
}
