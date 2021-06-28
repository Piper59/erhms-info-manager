using Epi;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Data
{
    public class ViewCollectionView : TypedListCollectionView<View>
    {
        public Project Project { get; }

        public ViewCollectionView(Project project)
        {
            Project = project;
        }

        public async Task InitializeAsync()
        {
            SourceCollection.Clear();
            SourceCollection.AddRange(await Task.Run(() =>
            {
                Project.LoadViews();
                return Project.Views.Cast<View>().ToList();
            }));
            Refresh();
        }
    }
}
