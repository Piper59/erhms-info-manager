using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ERHMS.DataAccess;


namespace ERHMS.WPF.ViewModel
{
    public class LocationViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }
        
        private Domain.Location currentLocation;
        public Domain.Location CurrentLocation
        {
            get { return currentLocation; }
            private set
            {
                Set(ref currentLocation, value);
                Latitude = currentLocation.Latitude != null ? (double)currentLocation.Latitude : 0;
                Longitude = currentLocation.Longitude != null ? (double)currentLocation.Longitude : 0;
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        public LocationViewModel()
        {
            DataContext dbContext = new DataContext();

            CurrentLocation = dbContext.Locations.Create();
        }

        public LocationViewModel(Domain.Location location)
        {
            CurrentLocation = location;

            Messenger.Default.Send(new NotificationMessage<Tuple<double, double>>(new Tuple<double, double>(Latitude, Longitude), "CenterMap"));

            SaveCommand = new RelayCommand(() =>
            {
                DataContext dbContext = new DataContext();
                dbContext.Locations.Save(CurrentLocation);
                
                Messenger.Default.Send(new NotificationMessage<string>("Location has been saved.", "ShowSuccessMessage"));

            });
        }

        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set
            {
                Set(ref latitude, value);
                CurrentLocation.Latitude = value;
                RaisePropertyChanged("Latitude");
                RaisePropertyChanged("MapLocations");
            }
        }
        private double longitude;
        public double Longitude
        {
            get { return longitude; }
            set
            {
                Set(ref longitude, value);
                CurrentLocation.Longitude = value;
                RaisePropertyChanged("Longitude");
                RaisePropertyChanged("MapLocations");
            }
        }

        public ObservableCollection<Microsoft.Maps.MapControl.WPF.Location> MapLocations
        {
            get
            {
                var mapLocations = new ObservableCollection<Microsoft.Maps.MapControl.WPF.Location>();

                if (Latitude != 0.0 && Longitude != 0.0)
                    mapLocations.Add(new Microsoft.Maps.MapControl.WPF.Location(double.Parse(Latitude.ToString()), double.Parse(Longitude.ToString())));

                return mapLocations;
            }
        }
    }
}
