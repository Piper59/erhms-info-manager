using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Presentation.Views
{
    public partial class LocationDetailView : UserControl
    {
        public LocationDetailView()
        {
            InitializeComponent();
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Location location = Map.ViewportPointToLocation(e.GetPosition(Map));
            Messenger.Default.Send(new LocateMessage(location));
            e.Handled = true;
        }
    }
}
