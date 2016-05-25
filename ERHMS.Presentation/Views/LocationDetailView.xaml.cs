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

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataContext.SetCoordinates(Map.ViewportPointToLocation(e.GetPosition(Map)));
            e.Handled = true;
        }
    }
}
