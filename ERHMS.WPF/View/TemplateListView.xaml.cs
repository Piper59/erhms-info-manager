using ERHMS.WPF.ViewModel;

namespace ERHMS.WPF.View
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
