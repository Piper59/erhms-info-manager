using ERHMS.Common.ComponentModel;

namespace ERHMS.Desktop.ViewModels
{
    public class ProgressViewModel : ObservableObject
    {
        private string lead;
        public string Lead
        {
            get { return lead; }
            set { SetProperty(ref lead, value); }
        }

        private string body;
        public string Body
        {
            get { return body; }
            set { SetProperty(ref body, value); }
        }
    }
}
