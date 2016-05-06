using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class RosterViewModel : ViewModelBase
    {
        public Roster Roster { get; private set; }
        public Responder Responder { get; private set; }

        public RosterViewModel(Roster roster, Responder responder)
        {
            Roster = roster;
            Responder = responder;
        }
    }
}
