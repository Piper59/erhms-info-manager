using ERHMS.Common;
using System;

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

        private bool complete;
        public bool Complete
        {
            get
            {
                return complete;
            }
            set
            {
                SetProperty(ref complete, value);
                if (value)
                {
                    OnCompleted();
                }
            }
        }

        public event EventHandler Completed;

        private void OnCompleted()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }
    }
}
