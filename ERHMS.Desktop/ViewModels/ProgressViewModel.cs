using ERHMS.Common;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        public string TaskName { get; }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        public ProgressViewModel(string taskName)
        {
            TaskName = taskName;
        }
    }
}
