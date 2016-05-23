using Epi;

namespace ERHMS.Presentation.Messages
{
    public class RefreshDataMessage
    {
        public string ProjectPath { get; private set; }
        public string ViewName { get; private set; }

        public RefreshDataMessage(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public RefreshDataMessage(View view)
            : this(view.Project.FilePath, view.Name) { }
    }
}
