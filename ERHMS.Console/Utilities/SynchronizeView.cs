using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;

namespace ERHMS.Console.Utilities
{
    public class SynchronizeView : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public bool Tree { get; }

        public SynchronizeView(string projectPath, string viewName, bool tree)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            Tree = tree;
        }

        public SynchronizeView(string projectPath, string viewName)
            : this(projectPath, viewName, true) { }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (Tree)
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
