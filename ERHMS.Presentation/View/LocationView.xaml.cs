using ERHMS.Utility;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ERHMS.Presentation.GeocodeService;
using ERHMS.Presentation.SearchService;
using ERHMS.Presentation.ImageryService;
using ERHMS.Presentation.RouteService;
using ERHMS.Presentation.ViewModel;
using ERHMS.Domain;

namespace ERHMS.Presentation.View
{
    /// <summary>
    /// Interaction logic for LocationView.xaml
    /// </summary>
    public partial class LocationView : UserControl
    {
        public LocationView(Incident incident)
        {
            InitializeComponent();

            LocationMap.CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Instance.MapLicenseKey);

            DataContext = new LocationViewModel(incident);
        }

        public LocationView(Domain.Location location)
        {
            InitializeComponent();

            LocationMap.CredentialsProvider = new ApplicationIdCredentialsProvider(Settings.Instance.MapLicenseKey);

            DataContext = new LocationViewModel(location);

            CenterMap(new Microsoft.Maps.MapControl.WPF.Location((double)location.Longitude, (double)location.Latitude));
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determine the location to place the pushpin at on the map.
            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(LocationMap);

            //Convert the mouse coordinates to a location on the map
            Microsoft.Maps.MapControl.WPF.Location pinLocation = LocationMap.ViewportPointToLocation(mousePosition);

            Messenger.Default.Send(new NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>(pinLocation, "UpdateMapLocation"));


        }

        private Point ctxMapPosition;
        private void LocationMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ctxMapPosition = e.GetPosition(LocationMap);
        }

        private void ZoomIn()
        {
            if (LocationMap.ZoomLevel < 20)
                LocationMap.ZoomLevel++;
        }
        private void ZoomOut()
        {
            if (LocationMap.ZoomLevel > 1)
                LocationMap.ZoomLevel--;
        }

        private void ctxMapZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Maps.MapControl.WPF.Location zoomLocation = LocationMap.ViewportPointToLocation(ctxMapPosition);
            LocationMap.Center = zoomLocation;

            ZoomIn();
        }
        private void ctxMapZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Maps.MapControl.WPF.Location zoomLocation = LocationMap.ViewportPointToLocation(ctxMapPosition);
            LocationMap.Center = zoomLocation;

            ZoomIn();
        }
        private void ctxMapDropPin_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Maps.MapControl.WPF.Location pinLocation = LocationMap.ViewportPointToLocation(ctxMapPosition);

            Messenger.Default.Send(new NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>(pinLocation, "UpdateMapLocation"));
        }

        private void btnMapZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }
        private void btnMapZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void btnUpdateMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Maps.MapControl.WPF.Location pinLocation = new Microsoft.Maps.MapControl.WPF.Location(double.Parse(txtLatitude.Text), double.Parse(txtLongitude.Text));

                CenterMap(pinLocation);

                Messenger.Default.Send(new NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>(pinLocation, "UpdateMapLocation"));
            }
            catch (Exception)
            {
                Messenger.Default.Send(new NotificationMessage<string>("An error occurred. Please verify the latitude and longitude coordinates are in the correct format (e.g. -79.84125).", "ShowErrorMessage"));
            }
        }
        private void btnGeocode_Click(object sender, RoutedEventArgs e)
        {
            double[] location = GeocodeAddress(txtAddress.Text);
            if (location != null)
            {
                txtLatitude.Text = location[0].ToString();
                txtLongitude.Text = location[1].ToString();

                Microsoft.Maps.MapControl.WPF.Location pinLocation = new Microsoft.Maps.MapControl.WPF.Location(location[0], location[1]);

                Messenger.Default.Send(new NotificationMessage<Microsoft.Maps.MapControl.WPF.Location>(pinLocation, "UpdateMapLocation"));

                CenterMap(pinLocation);
            }
            else
            {
                Messenger.Default.Send(new NotificationMessage<string>("The specified address was not found.", "ShowErrorMessage"));
            }
        }

        private void CenterMap(Microsoft.Maps.MapControl.WPF.Location location)
        {
            LocationMap.Center = location;
            LocationMap.ZoomLevel = 15;
        }

        private double[] GeocodeAddress(string address)
        {
            double[] results = { 0.0, 0.0 };
            string key = Settings.Instance.MapLicenseKey;
            GeocodeRequest geocodeRequest = new GeocodeRequest();

            // Set the credentials using a valid Bing Maps key
            geocodeRequest.Credentials = new Microsoft.Maps.MapControl.WPF.Credentials();
            geocodeRequest.Credentials.ApplicationId = key;

            // Set the full address query
            geocodeRequest.Query = address;

            // Set the options to only return high confidence results 
            ConfidenceFilter[] filters = new ConfidenceFilter[1];
            filters[0] = new ConfidenceFilter();
            filters[0].MinimumConfidence = GeocodeService.Confidence.High;

            // Add the filters to the options
            GeocodeOptions geocodeOptions = new GeocodeOptions();
            geocodeOptions.Filters = filters;
            geocodeRequest.Options = geocodeOptions;

            // Make the geocode request
            try
            {
                GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

                if (geocodeResponse.Results.Length > 0)
                {
                    results[0] = geocodeResponse.Results[0].Locations[0].Latitude;
                    results[1] = geocodeResponse.Results[0].Locations[0].Longitude;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                Messenger.Default.Send(new NotificationMessage<string>("Error while searching address.  Please check criteria.  Details: " + e.Message + ".", "ShowErrorMessage"));
            }
            return results;
        }
    }
}
