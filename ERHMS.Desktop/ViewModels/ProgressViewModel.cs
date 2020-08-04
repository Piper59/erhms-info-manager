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

        private string progress;
        public string Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }
    }
}
