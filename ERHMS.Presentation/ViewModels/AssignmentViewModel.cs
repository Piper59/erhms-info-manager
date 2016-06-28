using Epi;
using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentViewModel : ViewModelBase
    {
        public Assignment Assignment { get; private set; }
        public Link<View> View { get; private set; }
        public Responder Responder { get; private set; }

        public string IncidentId
        {
            get { return View.IncidentId; }
        }

        public string IncidentName
        {
            get { return View.IncidentName; }
        }

        public AssignmentViewModel(Assignment assignment, Link<View> view, Responder responder)
        {
            Assignment = assignment;
            View = view;
            Responder = responder;
        }
    }
}
