using Epi;
using ERHMS.Desktop.ViewModels.Collections;
using System.Linq;
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
            Views = new ViewCollectionViewModel();
        }

        public async Task InitializeAsync()
        {
            await Task.Run(Value.LoadViews);
            await Views.InitializeAsync(Value.Views.Cast<View>());
        }
    }
}
