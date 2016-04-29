using Microsoft.Maps.MapControl.WPF;

namespace ERHMS.Presentation.Messages
{
    public class LocateMessage
    {
        public Location Location { get; private set; }

        public LocateMessage(Location location)
        {
            Location = location;
        }
    }
}
