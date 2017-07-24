using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Location : Entity
    {
        public string LocationId
        {
            get { return GetProperty<string>(nameof(LocationId)); }
            set { SetProperty(nameof(LocationId), value); }
        }

        public string IncidentId
        {
            get { return GetProperty<string>(nameof(IncidentId)); }
            set { SetProperty(nameof(IncidentId), value); }
        }

        private Incident incident;
        public Incident Incident
        {
            get { return incident; }
            set { SetProperty(nameof(Incident), ref incident, value); }
        }

        public string Name
        {
            get { return GetProperty<string>(nameof(Name)); }
            set { SetProperty(nameof(Name), value); }
        }

        public string Description
        {
            get { return GetProperty<string>(nameof(Description)); }
            set { SetProperty(nameof(Description), value); }
        }

        public string Address
        {
            get { return GetProperty<string>(nameof(Address)); }
            set { SetProperty(nameof(Address), value); }
        }

        public double? Latitude
        {
            get { return GetProperty<double?>(nameof(Latitude)); }
            set { SetProperty(nameof(Latitude), value); }
        }

        public double? Longitude
        {
            get { return GetProperty<double?>(nameof(Longitude)); }
            set { SetProperty(nameof(Longitude), value); }
        }
    }
}
