using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class SynchronizeDataTables : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }

        public SynchronizeDataTables(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            view.SynchronizeDataTables();
        }
    }
}
