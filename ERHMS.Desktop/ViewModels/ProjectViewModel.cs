using Epi;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

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

        private ProjectViewModel(Project value)
        {
            Value = value;
        }

        private async Task InitializeAsync()
        {
            Views = await ViewCollectionViewModel.CreateAsync(Value);
        }
    }
}
