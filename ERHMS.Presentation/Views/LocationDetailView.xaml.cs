using ERHMS.Presentation.ViewModels;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Views
{
    public partial class LocationDetailView : UserControl
    {
        public new LocationDetailViewModel DataContext
        {
            get { return (LocationDetailViewModel)base.DataContext; }
        }

        public LocationDetailView()
        {
            InitializeComponent();
        }

        private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataContext.Target = Map.ViewportPointToLocation(e.GetPosition(Map));
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataContext.SetCoordinates(Map.ViewportPointToLocation(e.GetPosition(Map)));
            DataContext.SaveState();
            e.Handled = true;
        }
    }
}
