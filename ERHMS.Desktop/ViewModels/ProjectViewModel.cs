using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        public Project Value { get; }
        public ViewCollectionViewModel Views { get; }

        public ICommand RefreshCommand { get; }

        public ProjectViewModel(Project value)
        {
            Value = value;
            Views = new ViewCollectionViewModel(value);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            await Views.InitializeAsync();
        }

        public async Task RefreshAsync()
        {
            await ServiceLocator.Resolve<IProgressService>().RunAsync(
                ResXResources.Lead_RefreshingProject,
                async () =>
                {
                    await Views.RefreshAsync();
                });
        }
    }
}
