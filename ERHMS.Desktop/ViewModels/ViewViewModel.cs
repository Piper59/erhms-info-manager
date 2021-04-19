using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel
    {
        public static async Task<ViewViewModel> CreateAsync(View value)
        {
            ViewViewModel result = new ViewViewModel(value);
            await result.InitializeAsync();
            return result;
        }

        public View Value { get; }
        public RecordCollectionViewModel Records { get; private set; }

        public ICommand GoToProjectCommand { get; }

        private ViewViewModel(View value)
        {
            Value = value;
            GoToProjectCommand = new AsyncCommand(GoToProjectAsync);
        }

        private async Task InitializeAsync()
        {
            Records = await RecordCollectionViewModel.CreateAsync(Value);
        }

        public async Task GoToProjectAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(Value.Project.FilePath);
        }
    }
}
