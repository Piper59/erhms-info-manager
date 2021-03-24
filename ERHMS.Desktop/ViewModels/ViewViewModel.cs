using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ViewModel
    {
        public View Value { get; }
        public RecordCollectionViewModel Records { get; }

        public ICommand GoToProjectCommand { get; }

        public ViewViewModel(View value)
        {
            Value = value;
            Records = new RecordCollectionViewModel(value);
            GoToProjectCommand = new AsyncCommand(GoToProjectAsync);
        }

        public async Task InitializeAsync()
        {
            await Records.InitializeAsync();
        }

        public async Task GoToProjectAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(Value.Project.FilePath);
        }
    }
}
