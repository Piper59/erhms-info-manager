using Epi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Data
{
    public class ViewCollectionView : ListCollectionView<View>
    {
        public static async Task<ViewCollectionView> CreateAsync(Project project)
        {
            ViewCollectionView result = new ViewCollectionView(project);
            await result.InitializeAsync();
            return result;
        }

        public Project Project { get; }

        private ViewCollectionView(Project project)
            : base(new List<View>())
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
