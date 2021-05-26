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

        public ICommand OpenLocationCommand { get; }
        public ICommand GoToHelpCommand { get; }

        private ProjectViewModel(Project value)
        {
            Value = value;
            OpenLocationCommand = new SyncCommand(OpenLocation);
            GoToHelpCommand = Command.Null;
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
