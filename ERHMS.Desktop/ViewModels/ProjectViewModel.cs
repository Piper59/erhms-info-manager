using ERHMS.Common;
using ERHMS.EpiInfo.Projects;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public Project Project { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
        }
    }
}
