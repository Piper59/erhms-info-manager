using ERHMS.Common;
using ERHMS.Desktop.Infrastructure.ViewModels;

namespace ERHMS.Desktop.ViewModels
{
    public class IntegrationViewModel : ViewModel
    {
        private ViewModel content;
        public ViewModel Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Instance.Debug($"Setting integration content: {value}");
                SetProperty(ref content, value);
            }
        }
    }
}
