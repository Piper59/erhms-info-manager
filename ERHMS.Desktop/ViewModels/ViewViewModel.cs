using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels.Collections;
using System.Diagnostics;
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
        public Project Project => Value.Project;
        public RecordCollectionViewModel Records { get; private set; }

        public ICommand GoToHelpCommand { get; }
        public ICommand GoToProjectCommand { get; }
        public ICommand OpenProjectLocationCommand { get; }

        private ViewViewModel(View value)
        {
            Value = value;
            GoToHelpCommand = Command.Null;
            GoToProjectCommand = new AsyncCommand(GoToProjectAsync);
            OpenProjectLocationCommand = new SyncCommand(OpenProjectLocation);
        }

        private async Task InitializeAsync()
        {
            Records = await RecordCollectionViewModel.CreateAsync(Value);
        }

        public async Task GoToProjectAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(Task.FromResult(Project));
        }

        public void OpenProjectLocation()
        {
            Process.Start(Project.Location)?.Dispose();
        }
    }
}
