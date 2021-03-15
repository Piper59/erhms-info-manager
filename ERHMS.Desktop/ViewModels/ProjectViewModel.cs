using Epi;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ViewModel
    {
        public Project Value { get; }
        public ViewCollectionViewModel Views { get; private set; }

        public ProjectViewModel(Project value)
        {
            Value = value;
        }

        public async Task InitializeAsync()
        {
            Views = new ViewCollectionViewModel(await Task.Run(() => Value.Views.Cast<View>().ToList()));
        }
    }
}
