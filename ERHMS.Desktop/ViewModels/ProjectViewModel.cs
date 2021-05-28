using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel
    {
        public static async Task<ProjectViewModel> CreateAsync(Project value)
        {
            ProjectViewModel result = new ProjectViewModel(value);
            await result.InitializeAsync();
            return result;
        }

        public Project Value { get; }
        public ViewCollectionViewModel Views { get; private set; }
        public CanvasCollectionViewModel Canvases { get; private set; }
        public PgmCollectionViewModel Pgms { get; private set; }
        public MapCollectionViewModel Maps { get; private set; }

        public ICommand GoToHelpCommand { get; }
        public ICommand OpenLocationCommand { get; }

        private ProjectViewModel(Project value)
        {
            Value = value;
            GoToHelpCommand = Command.Null;
            OpenLocationCommand = new SyncCommand(OpenLocation);
        }

        private async Task InitializeAsync()
        {
            Views = await ViewCollectionViewModel.CreateAsync(Value);
            Canvases = await CanvasCollectionViewModel.CreateAsync(Value);
            Pgms = await PgmCollectionViewModel.CreateAsync(Value);
            Maps = await MapCollectionViewModel.CreateAsync(Value);
        }

        public void OpenLocation()
        {
            Process.Start(Value.Location)?.Dispose();
        }
    }
}
