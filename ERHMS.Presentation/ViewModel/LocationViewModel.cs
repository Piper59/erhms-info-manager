using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ERHMS.DataAccess;
using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModel
{
    public class LocationViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { Set(() => Title, ref title, value); }
        }
        
        private Domain.Location currentLocation;
        public Domain.Location CurrentLocation
        {
            get { return currentLocation; }
            private set
            {
                Set(() => CurrentLocation, ref currentLocation, value);
                Latitude = currentLocation.Latitude != null ? (double)currentLocation.Latitude : 0;
                Longitude = currentLocation.Longitude != null ? (double)currentLocation.Longitude : 0;
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        public LocationViewModel(Incident incident)
        {
            CurrentLocation = App.Current.DataContext.Locations.Create();
            CurrentLocation.IncidentId = incident.IncidentId;

            SaveCommand = new RelayCommand(() =>
            {
                App.Current.DataContext.Locations.Save(CurrentLocation);

                Messenger.Default.Send(new NotificationMessage<string>(CurrentLocation.IncidentId, "RefreshLocations"));
                
                Messenger.Default.Send(new NotificationMessage<string>("Location has been saved.", "ShowSuccessMessage"));
            });

            //used for updating the location on the map
            Messenger.Default.Register<NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>>(this, (msg) =>
            {
                if (msg.Notification == "UpdateMapLocation")
                {
                    //update the latitude and longitude
                    Latitude = msg.Content.Latitude;
                    Longitude = msg.Content.Longitude;
                }
            });
        }

        public LocationViewModel(Domain.Location location)
        {
            CurrentLocation = location;
            
            SaveCommand = new RelayCommand(() =>
            {
                App.Current.DataContext.Locations.Save(CurrentLocation);

                Messenger.Default.Send(new NotificationMessage<string>(CurrentLocation.IncidentId, "RefreshLocations"));

                Messenger.Default.Send(new NotificationMessage<string>("Location has been saved.", "ShowSuccessMessage"));
            });

            //used for updating the location on the map
            Messenger.Default.Register<NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>>(this, (msg) =>
            {
                if (msg.Notification == "UpdateMapLocation")
                {
                    //update the latitude and longitude
                    Latitude = msg.Content.Latitude;
                    Longitude = msg.Content.Longitude;
                }
            });
        }

        private double latitude;
        public double Latitude
        {
            get { return latitude; }
            set
            {
                Set(() => Latitude, ref latitude, value);
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
                Set(() => Longitude, ref longitude, value);
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
