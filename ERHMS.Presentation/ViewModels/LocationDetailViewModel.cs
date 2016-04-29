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

        private bool updating;

        public Location Location { get; private set; }
        public CredentialsProvider CredentialsProvider { get; private set; }

        private Coordinates center;
        public Coordinates Center
        {
            get { return center; }
            set { Set(() => Center, ref center, value); }
        }

        public ObservableCollection<Coordinates> Pins { get; private set; }

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
                Center = new Coordinates(location.Latitude.Value, location.Longitude.Value);
                Pins.Add(new Coordinates(Center));
                ZoomLevel = PinnedZoomLevel;
            }
            else
            {
                Center = new Coordinates(38.904722, -77.016389);
                ZoomLevel = UnpinnedZoomLevel;
            }
            SaveCommand = new RelayCommand(Save);
            LocateCommand = new RelayCommand(Locate, HasAddress);
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
                    if (!updating && Location.Latitude.HasValue && Location.Longitude.HasValue)
                    {
                        updating = true;
                        Center = new Coordinates(Location.Latitude.Value, Location.Longitude.Value);
                        Pins.Clear();
                        Pins.Add(new Coordinates(Center));
                        updating = false;
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
            // TODO: Geocode, pan, and pin
        }

        public void Save()
        {
            // TODO: Validate fields
            DataContext.Locations.Save(Location);
            UpdateTitle();
            Messenger.Default.Send(new RefreshMessage<Location>());
        }
    }
}
