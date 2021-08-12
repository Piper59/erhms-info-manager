using Epi;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel
    {
        public static async Task<ViewViewModel> CreateAsync(View view)
        {
            ViewViewModel result = new ViewViewModel(view);
            await result.InitializeAsync();
            return result;
        }

        public View View { get; }
        public Project Project => View.Project;
        public RecordCollectionViewModel Records { get; private set; }

        private ViewViewModel(View view)
        {
            View = view;
        }

        private async Task InitializeAsync()
        {
            Records = await RecordCollectionViewModel.CreateAsync(View);
        }
    }
}
