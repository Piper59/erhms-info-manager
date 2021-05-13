using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class DeleteView : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public bool Tree { get; }

        public DeleteView(string projectPath, string viewName, bool tree)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            Tree = tree;
        }

        public DeleteView(string projectPath, string viewName)
            : this(projectPath, viewName, true) { }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (Tree)
            {
                project.DeleteViewTree(view);
            }
            else
            {
                project.DeleteView(view);
            }
        }
    }
}
