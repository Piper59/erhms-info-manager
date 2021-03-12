using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class DeleteView : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }

        public DeleteView(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            project.DeleteViewTree(view);
        }
    }
}
