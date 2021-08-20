using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class DeletePage : Utility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string PageName { get; }

        public DeletePage(string projectPath, string viewName, string pageName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            PageName = pageName;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            Page page = view.GetPageByName(PageName);
            project.DeletePage(page);
        }
    }
}
