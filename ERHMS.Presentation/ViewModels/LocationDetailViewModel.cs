using ERHMS.Presentation.GeocodeService;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Coordinates = Microsoft.Maps.MapControl.WPF.Location;
using Location = ERHMS.Domain.Location;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationDetailViewModel : DocumentViewModel
    {
        private const double UnpinnedZoomLevel = 6.0;
        private const double PinnedZoomLevel = 12.0;

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

        public ObservableCollection<Coordinates> Pins { get; private set; }

        private Coordinates center;
        public Coordinates Center
        {
            get { return center; }
            set { Set(() => Center, ref center, value); }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { Set(() => ZoomLevel, ref zoomLevel, value); }
        }

        public RelayCommand LocateCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public LocationDetailViewModel(Location location)
        {
            Location = location;
            location.PropertyChanged += Location_PropertyChanged;
            UpdateTitle();
            CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Default.MapLicenseKey);
            Pins = new ObservableCollection<Coordinates>();
            if (location.Latitude.HasValue && location.Longitude.HasValue)
            {
                Coordinates pin = new Coordinates(location.Latitude.Value, location.Longitude.Value);
                Pins.Add(pin);
                Center = new Coordinates(pin);
                ZoomLevel = PinnedZoomLevel;
            }
            else
            {
                Center = new Coordinates(38.904722, -77.016389);
                ZoomLevel = UnpinnedZoomLevel;
            }
            SaveCommand = new RelayCommand(Save);
            LocateCommand = new RelayCommand(Locate, HasAddress);
            Messenger.Default.Register<LocateMessage>(this, OnMapMessage);
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Address":
                    LocateCommand.RaiseCanExecuteChanged();
                    break;
                case "Latitude":
                case "Longitude":
                    Pins.Clear();
                    if (Location.Latitude.HasValue && Location.Longitude.HasValue)
                    {
                        Pins.Add(new Coordinates(Location.Latitude.Value, Location.Longitude.Value));
                    }
                    break;
            }
        }

        public bool HasAddress()
        {
            return !string.IsNullOrEmpty(Location.Address);
        }

        private void UpdateTitle()
        {
            if (Location.New)
            {
                Title = "New Location";
            }
            else
            {
                Title = Location.Name;
            }
        }

        public void Locate()
        {
            GeocodeRequest request = new GeocodeRequest
            {
                Credentials = new Credentials
                {
                    ApplicationId = Settings.Default.MapLicenseKey
                },
                Query = Location.Address,
                Options = new GeocodeOptions
                {
                    Filters = new ConfidenceFilter[]
                    {
                        new ConfidenceFilter
                        {
                            MinimumConfidence = Confidence.High
                        }
                    }
                }
            };
            GeocodeServiceClient client = new GeocodeServiceClient();
            GeocodeResponse response = client.Geocode(request);
            if (response.Results.Length > 0)
            {
                GeocodeLocation result = response.Results[0].Locations[0];
                Location.Latitude = null;
                Location.Longitude = null;
                Location.Latitude = result.Latitude;
                Location.Longitude = result.Longitude;
                Center = new Coordinates(Location.Latitude.Value, Location.Longitude.Value);
                ZoomLevel = PinnedZoomLevel;
            }
            else
            {
                // TODO: Display failure notification
            }
        }

        public void Save()
        {
            // TODO: Validate fields
            DataContext.Locations.Save(Location);
            UpdateTitle();
            Messenger.Default.Send(new RefreshMessage<Location>());
        }

        private void OnMapMessage(LocateMessage msg)
        {
            Location.Latitude = null;
            Location.Longitude = null;
            Location.Latitude = msg.Location.Latitude;
            Location.Longitude = msg.Location.Longitude;
        }
    }
}
