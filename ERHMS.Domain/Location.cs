using System;

namespace ERHMS.Domain
{
    public class Location : Link
    {
        public string LocationId
        {
            get { return GetProperty<string>(nameof(LocationId)); }
            set { SetProperty(nameof(LocationId), value); }
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

        public Location()
        {
            LocationId = Guid.NewGuid().ToString();
        }
    }
}
