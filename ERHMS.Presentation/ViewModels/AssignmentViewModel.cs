using Epi;
using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentViewModel : ViewModelBase
    {
        public Assignment Assignment { get; private set; }
        public DeepLink<View> ViewDeepLink { get; private set; }
        public Responder Responder { get; private set; }

        public AssignmentViewModel(Assignment assignment, DeepLink<View> viewDeepLink, Responder responder)
        {
            Assignment = assignment;
            ViewDeepLink = viewDeepLink;
            Responder = responder;
        }
    }
}
