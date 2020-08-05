using ERHMS.Common;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        private string taskName;
        public string TaskName
        {
            get { return taskName; }
            set { SetProperty(ref taskName, value); }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
    }
}
