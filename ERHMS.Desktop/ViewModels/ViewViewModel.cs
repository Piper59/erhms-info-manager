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
        public View Value { get; }
        public Project Project => Value.Project;
        public RecordCollectionViewModel Records { get; }

        public ICommand GoToHelpCommand { get; }
        public ICommand GoToProjectCommand { get; }
        public ICommand OpenProjectLocationCommand { get; }

        public ViewViewModel(View value)
        {
            Value = value;
            Records = new RecordCollectionViewModel(value);
            GoToHelpCommand = Command.Null;
            GoToProjectCommand = new AsyncCommand(GoToProjectAsync);
            OpenProjectLocationCommand = new SyncCommand(OpenProjectLocation);
        }

        public async Task InitializeAsync()
        {
            await Records.InitializeAsync();
        }

        public async Task GoToProjectAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(() => Task.FromResult(Project));
        }

        public void OpenProjectLocation()
        {
            Process.Start(Project.Location)?.Dispose();
        }
    }
}
