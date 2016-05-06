using Epi;
using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignmentViewModel : ViewModelBase
    {
        public Assignment Assignment { get; private set; }
        public View View { get; private set; }
        public Responder Responder { get; private set; }

        public AssignmentViewModel(Assignment assignment, View view, Responder responder)
        {
            Assignment = assignment;
            View = view;
            Responder = responder;
        }
    }
}
