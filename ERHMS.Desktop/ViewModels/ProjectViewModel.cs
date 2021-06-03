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
        public Project Value { get; }
        public ViewCollectionViewModel Views { get; }
        public CanvasCollectionViewModel Canvases { get; }
        public PgmCollectionViewModel Pgms { get; }
        public MapCollectionViewModel Maps { get; }

        public ICommand GoToHelpCommand { get; }
        public ICommand OpenLocationCommand { get; }

        public ProjectViewModel(Project value)
        {
            Value = value;
            Views = new ViewCollectionViewModel(value);
            Canvases = new CanvasCollectionViewModel(value);
            Pgms = new PgmCollectionViewModel(value);
            Maps = new MapCollectionViewModel(value);
            GoToHelpCommand = Command.Null;
            OpenLocationCommand = new SyncCommand(OpenLocation);
        }

        public async Task InitializeAsync()
        {
            await Views.InitializeAsync();
            await Canvases.InitializeAsync();
            await Pgms.InitializeAsync();
            await Maps.InitializeAsync();
        }

        public void OpenLocation()
        {
            Process.Start(Value.Location)?.Dispose();
        }
    }
}
