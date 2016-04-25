using ERHMS.Presentation.ViewModel;

namespace ERHMS.Presentation.View
{
    public partial class TemplateListView
    {
        public TemplateListView()
        {
            DataContext = new TemplateListViewModel();
            InitializeComponent();
        }
    }
}
