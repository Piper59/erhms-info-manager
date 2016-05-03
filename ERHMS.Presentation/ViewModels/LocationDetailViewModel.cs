using ERHMS.Presentation.GeocodeService;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Coordinates = Microsoft.Maps.MapControl.WPF.Location;
using Location = ERHMS.Domain.Location;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationDetailViewModel : ViewModelBase
    {
        private const double UnpinnedZoomLevel = 6.0;
        private const double PinnedZoomLevel = 12.0;

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

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

        public ObservableCollection<Coordinates> Pins { get; private set; }

        public RelayCommand LocateCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public LocationDetailViewModel(Location location)
        {
            Location = location;
            location.PropertyChanged += Location_PropertyChanged;
            UpdateTitle();
            CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Default.MapLicenseKey);
            Pins = new ObservableCollection<Coordinates>();
            if (HasCoordinates())
            {
                Center = GetCoordinates();
                ZoomLevel = PinnedZoomLevel;
                Pins.Add(GetCoordinates());
            }
            else
            {
                Center = new Coordinates(38.904722, -77.016389);
                ZoomLevel = UnpinnedZoomLevel;
            }
            SaveCommand = new RelayCommand(Save);
            LocateCommand = new RelayCommand(Locate, HasAddress);
            Messenger.Default.Register<LocateMessage>(this, OnLocateMessage);
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
                    if (HasCoordinates())
                    {
                        Pins.Add(GetCoordinates());
                    }
                    break;
            }
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

        public bool HasAddress()
        {
            return !string.IsNullOrEmpty(Location.Address);
        }

        public bool HasCoordinates()
        {
            return Location.Latitude.HasValue && Location.Longitude.HasValue;
        }

        public Coordinates GetCoordinates()
        {
            return new Coordinates(Location.Latitude.Value, Location.Longitude.Value);
        }

        public void SetCoordinates(double latitude, double longitude)
        {
            Location.Latitude = null;
            Location.Longitude = null;
            Location.Latitude = latitude;
            Location.Longitude = longitude;
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
                SetCoordinates(result.Latitude, result.Longitude);
                Center = GetCoordinates();
                ZoomLevel = PinnedZoomLevel;
            }
            else
            {
                Messenger.Default.Send(new ToastMessage(NotificationType.Error, "Address could not be found."));
            }
        }

        public void Save()
        {
            // TODO: Validate fields
            DataContext.Locations.Save(Location);
            Messenger.Default.Send(new ToastMessage(NotificationType.Information, "Location has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Location>(Location.IncidentId));
            UpdateTitle();
        }

        private void OnLocateMessage(LocateMessage msg)
        {
            SetCoordinates(msg.Location.Latitude, msg.Location.Longitude);
        }
    }
}
