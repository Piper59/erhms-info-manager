using Epi;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Data
{
    public class ViewListCollectionView : ListCollectionView<View>
    {
        public static async Task<ViewListCollectionView> CreateAsync(Project project)
        {
            ViewListCollectionView result = new ViewListCollectionView(project);
            await result.InitializeAsync();
            return result;
        }

        public Project Project { get; }

        private ViewListCollectionView(Project project)
            : base()
        {
            Project = project;
        }

        private async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                Project.LoadViews();
                List.AddRange(Project.Views.Cast<View>());
            });
            Refresh();
        }
    }
}
