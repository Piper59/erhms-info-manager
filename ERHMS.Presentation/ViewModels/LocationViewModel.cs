using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Coordinates = Microsoft.Maps.MapControl.WPF.Location;
using Location = ERHMS.Domain.Location;

namespace ERHMS.Presentation.ViewModels
{
    public class LocationViewModel : DocumentViewModel
    {
        private const double ZoomLevelMin = 0.75;
        private const double ZoomLevelMax = 21.0;
        private const double ZoomLevelStep = 1.2;
        private const double ZoomLevelUnpinned = 6.0;
        private const double ZoomLevelPinned = 14.0;
        private static readonly Coordinates DistrictOfColumbia = new Coordinates(38.895, -77.036389);

        private static Coordinates LastCenter { get; set; }
        private static double? LastZoomLevel { get; set; }

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

        private Coordinates center;
        public Coordinates Center
        {
            get { return center; }
            set { SetProperty(nameof(Center), ref center, value); }
        }

        private Coordinates target;
        public Coordinates Target
        {
            get { return target; }
            set { SetProperty(nameof(Target), ref target, value); }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { SetProperty(nameof(ZoomLevel), ref zoomLevel, value); }
        }

        public ObservableCollection<Coordinates> Pins { get; private set; }

        public ICommand LocateCommand { get; private set; }
        public ICommand DropPinCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand CenterAndZoomInCommand { get; private set; }
        public ICommand CenterAndZoomOutCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public LocationViewModel(Location location)
        {
            Title = location.New ? "New Location" : location.Name;
            Location = location;
            AddDirtyCheck(location);
            CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Default.MapApplicationId);
            Pins = new ObservableCollection<Coordinates>();
            if (HasCoordinates())
            {
                Center = GetCoordinates();
                ZoomLevel = ZoomLevelPinned;
                Pins.Add(GetCoordinates());
                SaveState();
            }
            else
            {
                LoadState();
            }
            LocateCommand = new AsyncCommand(LocateAsync, CanLocate);
            DropPinCommand = new Command(DropPin);
            ZoomInCommand = new Command(ZoomIn, CanZoomIn);
            ZoomOutCommand = new Command(ZoomOut, CanZoomOut);
            CenterAndZoomInCommand = new Command(CenterAndZoomIn, CanZoomIn);
            CenterAndZoomOutCommand = new Command(CenterAndZoomOut, CanZoomOut);
            SaveCommand = new AsyncCommand(SaveAsync);
            location.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Location.Latitude) || e.PropertyName == nameof(Location.Longitude))
                {
                    Pins.Clear();
                    if (HasCoordinates())
                    {
                        Pins.Add(GetCoordinates());
                    }
                }
            };
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ZoomLevel))
                {
                    ZoomInCommand.OnCanExecuteChanged();
                    ZoomOutCommand.OnCanExecuteChanged();
                    CenterAndZoomInCommand.OnCanExecuteChanged();
                    CenterAndZoomOutCommand.OnCanExecuteChanged();
                }
            };
        }

        public void LoadState()
        {
            Center = LastCenter ?? DistrictOfColumbia;
            ZoomLevel = LastZoomLevel ?? ZoomLevelUnpinned;
        }

        public void SaveState()
        {
            LastCenter = Center;
            LastZoomLevel = ZoomLevel;
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

        public bool CanLocate()
        {
            return !string.IsNullOrWhiteSpace(Location.Address);
        }

        public async Task LocateAsync()
        {
            NameValueCollection query = HttpUtility.ParseQueryString("");
            query["userLocation"] = string.Format("{0},{1}", Center.Latitude, Center.Longitude);
            query["query"] = Location.Address;
            query["output"] = "xml";
            query["maxResults"] = "1";
            query["key"] = Settings.Default.MapApplicationId;
            WebRequest request = WebRequest.Create("http://dev.virtualearth.net/REST/v1/Locations?" + query.ToString());
            try
            {
                using (ServiceLocator.Busy.Begin())
                using (WebResponse response = request.GetResponse())
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(response.GetResponseStream());
                    XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
                    manager.AddNamespace("v1", "http://schemas.microsoft.com/search/local/ws/rest/v1");
                    XmlElement point = document.SelectSingleElement("/v1:Response/v1:ResourceSets/v1:ResourceSet/v1:Resources/v1:Location/v1:Point", manager);
                    if (point == null)
                    {
                        await ServiceLocator.Dialog.AlertAsync(Resources.LocationGeolocateEmpty);
                    }
                    else
                    {
                        double latitude = double.Parse(point.SelectSingleElement("v1:Latitude", manager).InnerText);
                        double longitude = double.Parse(point.SelectSingleElement("v1:Longitude", manager).InnerText);
                        SetCoordinates(latitude, longitude);
                        Center = GetCoordinates();
                        ZoomLevel = ZoomLevelPinned;
                        SaveState();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to locate address", ex);
                await ServiceLocator.Dialog.ShowErrorAsync(Resources.LocationGeolocateFailed, ex);
                ServiceLocator.Document.ShowSettings();
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

        public bool CanZoomIn()
        {
            return ZoomLevel < ZoomLevelMax;
        }

        public void ZoomIn()
        {
            SetZoomLevel(ZoomLevel + ZoomLevelStep);
        }

        public bool CanZoomOut()
        {
            return ZoomLevel > ZoomLevelMin;
        }

        public void ZoomOut()
        {
            SetZoomLevel(ZoomLevel - ZoomLevelStep);
        }

        public void CenterAndZoomIn()
        {
            if (Target != null)
            {
                Center = Target;
            }
            ZoomIn();
        }

        public void CenterAndZoomOut()
        {
            if (Target != null)
            {
                Center = Target;
            }
            ZoomOut();
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Location.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                await ServiceLocator.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            Context.Locations.Save(Location);
            ServiceLocator.Dialog.Notify(Resources.LocationSaved);
            ServiceLocator.Data.Refresh(typeof(Location));
            Title = Location.Name;
            Dirty = false;
            SaveState();
        }
    }
}
