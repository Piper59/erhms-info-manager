using ERHMS.Presentation.GeocodeService;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Coordinates = Microsoft.Maps.MapControl.WPF.Location;
using Location = ERHMS.Domain.Location;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationDetailViewModel : ViewModelBase
    {
        private const double UnpinnedZoomLevel = 6.0;
        private const double PinnedZoomLevel = 12.0;
        private static readonly Coordinates Washington = new Coordinates(38.904722, -77.016389);

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
            location.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(location.Address))
                {
                    LocateCommand.RaiseCanExecuteChanged();
                }
                else if (e.PropertyName == nameof(location.Latitude) || e.PropertyName == nameof(location.Longitude))
                {
                    Pins.Clear();
                    if (HasCoordinates())
                    {
                        Pins.Add(GetCoordinates());
                    }
                }
            };
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
                Center = Washington;
                ZoomLevel = UnpinnedZoomLevel;
            }
            SaveCommand = new RelayCommand(Save);
            LocateCommand = new RelayCommand(Locate, HasAddress);
            Messenger.Default.Register<LocateMessage>(this, OnLocateMessage);
        }

        private void UpdateTitle()
        {
            Title = Location.New ? "New Location" : Location.Name;
        }

        public bool HasAddress()
        {
            return !string.IsNullOrWhiteSpace(Location.Address);
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
                Messenger.Default.Send(new NotifyMessage("Address could not be found."));
            }
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Location.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                NotifyRequired(fields);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            DataContext.Locations.Save(Location);
            Messenger.Default.Send(new ToastMessage("Location has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Location>(Location.IncidentId));
            UpdateTitle();
        }

        private void OnLocateMessage(LocateMessage msg)
        {
            SetCoordinates(msg.Location.Latitude, msg.Location.Longitude);
        }
    }
}
