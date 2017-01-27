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
        private const double ZoomLevelMin = 0.75;
        private const double ZoomLevelMax = 21.0;
        private const double ZoomLevelIncrement = 1.2;
        private const double UnpinnedZoomLevel = 6.0;
        private const double PinnedZoomLevel = 14.0;
        private static readonly Coordinates DistrictOfColumbia = new Coordinates(38.904722, -77.016389);

        private static Coordinates LastCenter { get; set; }
        private static double? LastZoomLevel { get; set; }

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

        private Coordinates center;
        public Coordinates Center
        {
            get { return center; }
            set { Set(() => Center, ref center, value); }
        }

        private Coordinates target;
        public Coordinates Target
        {
            get { return target; }
            set { Set(() => Target, ref target, value); }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { Set(() => ZoomLevel, ref zoomLevel, value); }
        }

        public ObservableCollection<Coordinates> Pins { get; private set; }

        public RelayCommand LocateCommand { get; private set; }
        public RelayCommand DropPinCommand { get; private set; }
        public RelayCommand ZoomInCommand { get; private set; }
        public RelayCommand ZoomOutCommand { get; private set; }
        public RelayCommand CenterZoomInCommand { get; private set; }
        public RelayCommand CenterZoomOutCommand { get; private set; }
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
            location.PropertyChanged += OnDirtyCheckPropertyChanged;
            AfterClosed += (sender, e) =>
            {
                location.PropertyChanged -= OnDirtyCheckPropertyChanged;
            };
            UpdateTitle();
            CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Instance.MapLicenseKey);
            Pins = new ObservableCollection<Coordinates>();
            if (HasCoordinates())
            {
                Center = GetCoordinates();
                ZoomLevel = PinnedZoomLevel;
                Pins.Add(GetCoordinates());
                SaveState();
            }
            else
            {
                Center = LastCenter ?? DistrictOfColumbia;
                ZoomLevel = LastZoomLevel ?? UnpinnedZoomLevel;
            }
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ZoomLevel))
                {
                    ZoomInCommand.RaiseCanExecuteChanged();
                    ZoomOutCommand.RaiseCanExecuteChanged();
                }
            };
            LocateCommand = new RelayCommand(Locate, HasAddress);
            DropPinCommand = new RelayCommand(DropPin);
            ZoomInCommand = new RelayCommand(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut, CanZoomOut);
            CenterZoomInCommand = new RelayCommand(CenterZoomIn, CanZoomIn);
            CenterZoomOutCommand = new RelayCommand(CenterZoomOut, CanZoomOut);
            SaveCommand = new RelayCommand(Save);
        }

        private void UpdateTitle()
        {
            Title = Location.New ? "New Location" : Location.Name;
        }

        public void SaveState()
        {
            LastCenter = Center;
            LastZoomLevel = ZoomLevel;
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

        public void SetCoordinates(Coordinates coordinates)
        {
            SetCoordinates(coordinates.Latitude, coordinates.Longitude);
        }

        public void Locate()
        {
            GeocodeRequest request = new GeocodeRequest
            {
                Credentials = new Credentials
                {
                    ApplicationId = Settings.Instance.MapLicenseKey
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
                SaveState();
            }
            else
            {
                Messenger.Default.Send(new NotifyMessage("Address could not be found."));
            }
        }

        public void DropPin()
        {
            if (Target != null)
            {
                SetCoordinates(Target);
                SaveState();
            }
        }

        public bool CanZoomIn()
        {
            return ZoomLevel < ZoomLevelMax;
        }

        public bool CanZoomOut()
        {
            return ZoomLevel > ZoomLevelMin;
        }

        public void SetZoomLevel(double zoomLevel)
        {
            if (zoomLevel < ZoomLevelMin)
            {
                ZoomLevel = ZoomLevelMin;
            }
            else if (zoomLevel > ZoomLevelMax)
            {
                ZoomLevel = ZoomLevelMax;
            }
            else
            {
                ZoomLevel = zoomLevel;
            }
        }

        public void ZoomIn()
        {
            SetZoomLevel(ZoomLevel + ZoomLevelIncrement);
        }

        public void ZoomOut()
        {
            SetZoomLevel(ZoomLevel - ZoomLevelIncrement);
        }

        public void CenterZoomIn()
        {
            if (Target != null)
            {
                Center = Target;
            }
            ZoomIn();
        }

        public void CenterZoomOut()
        {
            if (Target != null)
            {
                Center = Target;
            }
            ZoomOut();
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
            Dirty = false;
            Messenger.Default.Send(new ToastMessage("Location has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Location>(Location.IncidentId));
            UpdateTitle();
            SaveState();
        }
    }
}
