using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ViewModel
    {
        public View Value { get; }
        public RecordCollectionViewModel Records { get; private set; }

        public ICommand GoToProjectCommand { get; }
        public ICommand RefreshCommand { get; }

        public ViewViewModel(View value)
        {
            Value = value;
            GoToProjectCommand = new AsyncCommand(GoToProjectAsync);
            RefreshCommand = new AsyncCommand(RefreshAsync);
        }

        public async Task InitializeAsync()
        {
            Records = new RecordCollectionViewModel(Value, await Task.Run(() =>
            {
                RecordRepository recordRepository = new RecordRepository(Value);
                return recordRepository.Select().ToList();
            }));
            await Records.InitializeAsync();
        }

        public async Task GoToProjectAsync()
        {
            await MainViewModel.Instance.GoToProjectAsync(Task.Run(() =>
            {
                return ProjectExtensions.Open(Value.Project.FilePath);
            }));
        }

        public async Task RefreshAsync()
        {
            await MainViewModel.Instance.GoToViewAsync(Task.Run(() =>
            {
                Project project = ProjectExtensions.Open(Value.Project.FilePath);
                return project.Views[Value.Name];
            }));
        }
    }
}
