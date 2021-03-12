using ERHMS.Desktop.Data;

namespace ERHMS.Desktop.Infrastructure.ViewModels
{
    public class SelectableViewModel : ViewModel, ISelectable
    {
        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set { SetProperty(ref selected, value); }
        }
    }
}
