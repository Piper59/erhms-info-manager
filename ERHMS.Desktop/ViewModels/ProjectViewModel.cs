using Epi;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        public Project Value { get; }
        public ViewCollectionViewModel Views { get; }

        public ProjectViewModel(Project value)
        {
            Value = value;
            Views = new ViewCollectionViewModel(value);
        }

        public async Task InitializeAsync()
        {
            await Views.InitializeAsync();
        }
    }
}
