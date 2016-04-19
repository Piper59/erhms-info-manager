using ERHMS.EpiInfo.Domain;

namespace ERHMS.Domain
{
    public class Location : TableEntity
    {
        public override string Guid
        {
            get
            {
                return GetProperty<string>("LocationId");
            }
            set
            {
                if (!SetProperty("LocationId", value))
                {
                    return;
                }
                OnPropertyChanged("Guid");
            }
        }

        public string LocationId
        {
            get { return GetProperty<string>("LocationId"); }
            set { SetProperty("LocationId", value); }
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

        public string Address
        {
            get { return GetProperty<string>("Address"); }
            set { SetProperty("Address", value); }
        }

        public double? Latitude
        {
            get { return GetProperty<double?>("Latitude"); }
            set { SetProperty("Latitude", value); }
        }

        public double? Longitude
        {
            get { return GetProperty<double?>("Longitude"); }
            set { SetProperty("Longitude", value); }
        }
    }
}
