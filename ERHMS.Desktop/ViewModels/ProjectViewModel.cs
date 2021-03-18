using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        public Project Value { get; }
        public ViewCollectionViewModel Views { get; private set; }

        public ICommand RefreshCommand { get; }

        public ProjectViewModel(Project value)
        {
            Value = value;
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            Views = new ViewCollectionViewModel(Value, await Task.Run(() =>
            {
                return Value.Views.Cast<View>().ToList();
            }));
            await Views.InitializeAsync();
        }

        public async Task RefreshAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(Task.Run(() =>
            {
                return ProjectExtensions.Open(Value.FilePath);
            }));
        }
    }
}
