using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Incident : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("IncidentId");
            }
            set
            {
                if (!SetProperty("IncidentId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string IncidentId
        {
            get { return GetProperty<string>("IncidentId"); }
            set { SetProperty("IncidentId", value); }
        }

        public string Name
        {
            get { return GetProperty<string>("Name"); }
            set { SetProperty("Name", value); }
        }

        public string Description
        {
            get { return GetProperty<string>("Description"); }
            set { SetProperty("Description", value); }
        }

        public Phase Phase
        {
            get { return (Phase)Enum.Parse(typeof(Phase), GetProperty<string>("Phase")); }
            set { SetProperty("Phase", value.ToString()); }
        }

        public DateTime? StartDate
        {
            get { return GetProperty<DateTime?>("StartDate"); }
            set { SetProperty("StartDate", value); }
        }

        public DateTime? EndDateEstimate
        {
            get { return GetProperty<DateTime?>("EndDateEstimate"); }
            set { SetProperty("EndDateEstimate", value); }
        }

        public DateTime? EndDateActual
        {
            get { return GetProperty<DateTime?>("EndDateActual"); }
            set { SetProperty("EndDateActual", value); }
        }
    }
}
