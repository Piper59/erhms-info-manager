using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;

namespace ERHMS.Console.Utilities
{
    public class SynchronizeView : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public bool Recursive { get; }

        public SynchronizeView(string projectPath, string viewName, bool recursive)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            Recursive = recursive;
        }

        public SynchronizeView(string projectPath, string viewName)
            : this(projectPath, viewName, true) { }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (Recursive)
            {
                project.CollectedData.SynchronizeViewTree(view);
            }
            else
            {
                project.CollectedData.SynchronizeView(view);
            }
        }
    }
}
