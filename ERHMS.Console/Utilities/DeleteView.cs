using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class DeleteView : Utility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public bool Recursive { get; }

        public DeleteView(string projectPath, string viewName, bool recursive)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            Recursive = recursive;
        }

        public DeleteView(string projectPath, string viewName)
            : this(projectPath, viewName, true) { }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (Recursive)
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
