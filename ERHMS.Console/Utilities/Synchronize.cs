using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class Synchronize : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public bool IncludeDescendants { get; }

        public Synchronize(string projectPath, string viewName)
            : this(projectPath, viewName, true) { }

        public Synchronize(string projectPath, string viewName, bool includeDescendants)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            IncludeDescendants = includeDescendants;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (IncludeDescendants)
            {
                view.SynchronizeTree();
            }
            else
            {
                view.Synchronize();
            }
        }
    }
}
