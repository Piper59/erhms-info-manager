using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Maps.MapControl.WPF.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using System.Xml;
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
        private static readonly Coordinates DistrictOfColumbia = new Coordinates(38.895, -77.036389);

        private static Coordinates LastCenter { get; set; }
        private static double? LastZoomLevel { get; set; }

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

        private Coordinates center;
        public Coordinates Center
        {
            get { return center; }
            set { Set(nameof(Center), ref center, value); }
        }

        private Coordinates target;
        public Coordinates Target
        {
            get { return target; }
            set { Set(nameof(Target), ref target, value); }
        }

        private double zoomLevel;
        public double ZoomLevel
        {
            get { return zoomLevel; }
            set { Set(nameof(ZoomLevel), ref zoomLevel, value); }
        }

        public ObservableCollection<Coordinates> Pins { get; private set; }

        public RelayCommand LocateCommand { get; private set; }
        public RelayCommand DropPinCommand { get; private set; }
        public RelayCommand ZoomInCommand { get; private set; }
        public RelayCommand ZoomOutCommand { get; private set; }
        public RelayCommand CenterAndZoomInCommand { get; private set; }
        public RelayCommand CenterAndZoomOutCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }

        public LocationDetailViewModel(IServiceManager services, Location location)
            : base(services)
        {
            Title = location.New ? "New Location" : location.Name;
            Location = location;
            CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Default.MapApplicationId);
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
                LoadState();
            }
            LocateCommand = new RelayCommand(Locate, HasAddress);
            DropPinCommand = new RelayCommand(DropPin);
            ZoomInCommand = new RelayCommand(ZoomIn, CanZoomIn);
            ZoomOutCommand = new RelayCommand(ZoomOut, CanZoomOut);
            CenterAndZoomInCommand = new RelayCommand(CenterAndZoomIn, CanZoomIn);
            CenterAndZoomOutCommand = new RelayCommand(CenterAndZoomOut, CanZoomOut);
            SaveCommand = new RelayCommand(Save);
            AddDirtyCheck(location);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ZoomLevel))
                {
                    ZoomInCommand.RaiseCanExecuteChanged();
                    ZoomOutCommand.RaiseCanExecuteChanged();
                    CenterAndZoomInCommand.RaiseCanExecuteChanged();
                    CenterAndZoomOutCommand.RaiseCanExecuteChanged();
                }
            };
            location.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Location.Address))
                {
                    LocateCommand.RaiseCanExecuteChanged();
                }
                else if (e.PropertyName == nameof(Location.Latitude) || e.PropertyName == nameof(Location.Longitude))
                {
                    Pins.Clear();
                    if (HasCoordinates())
                    {
                        Pins.Add(GetCoordinates());
                    }
                }
            };
        }

        public void SaveState()
        {
            LastCenter = Center;
            LastZoomLevel = ZoomLevel;
        }

        public void LoadState()
        {
            Center = LastCenter ?? DistrictOfColumbia;
            ZoomLevel = LastZoomLevel ?? UnpinnedZoomLevel;
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

        public void Locate()
        {
            NameValueCollection queryString = HttpUtility.ParseQueryString("");
            queryString["userLocation"] = string.Format("{0},{1}", Center.Latitude, Center.Longitude);
            queryString["query"] = Location.Address;
            queryString["output"] = "xml";
            queryString["maxResults"] = "1";
            queryString["key"] = Settings.Default.MapApplicationId;
            WebRequest request = WebRequest.Create("http://dev.virtualearth.net/REST/v1/Locations?" + queryString.ToString());
            try
            {
                using (new WaitCursor())
                using (WebResponse response = request.GetResponse())
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(response.GetResponseStream());
                    XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
                    manager.AddNamespace("v1", "http://schemas.microsoft.com/search/local/ws/rest/v1");
                    XmlElement point = document.SelectSingleElement("/v1:Response/v1:ResourceSets/v1:ResourceSet/v1:Resources/v1:Location/v1:Point", manager);
                    if (point == null)
                    {
                        MessengerInstance.Send(new AlertMessage
                        {
                            Message = "Address could not be found."
                        });
                    }
                    else
                    {
                        double latitude = double.Parse(point.SelectSingleElement("v1:Latitude", manager).InnerText);
                        double longitude = double.Parse(point.SelectSingleElement("v1:Longitude", manager).InnerText);
                        SetCoordinates(latitude, longitude);
                        Center = GetCoordinates();
                        ZoomLevel = PinnedZoomLevel;
                        SaveState();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to locate address", ex);
                Documents.ShowSettings("Failed to locate address. Please verify mapping settings.", ex);
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

        public void ZoomIn()
        {
            SetZoomLevel(ZoomLevel + ZoomLevelIncrement);
        }

        public void ZoomOut()
        {
            SetZoomLevel(ZoomLevel - ZoomLevelIncrement);
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

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Location.Name))
            {
                fields.Add("Name");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Context.Locations.Save(Location);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Location has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Location)));
            Title = Location.Name;
            SaveState();
            Dirty = false;
        }
    }
}
