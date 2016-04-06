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
        
        private Domain.Location selectedLocation;
        public Domain.Location SelectedLocation
        {
            get { return selectedLocation; }
            private set
            {
                Set(ref selectedLocation, value);
                Latitude = selectedLocation.Latitude != null ? (double)selectedLocation.Latitude : 0;
                Longitude = selectedLocation.Longitude != null ? (double)selectedLocation.Longitude : 0;
            }
        }

        public RelayCommand SaveCommand { get; private set; }

        public LocationViewModel()
        {
            SelectedLocation = App.GetDataContext().Locations.Create();
        }

        public LocationViewModel(Domain.Location location)
        {
            SelectedLocation = location;

            Messenger.Default.Send(new NotificationMessage<Tuple<double, double>>(new Tuple<double, double>(Latitude, Longitude), "CenterMap"));

            SaveCommand = new RelayCommand(() =>
            {
                App.GetDataContext().Locations.Save(SelectedLocation);
                
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
                SelectedLocation.Latitude = value;
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
                SelectedLocation.Longitude = value;
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
