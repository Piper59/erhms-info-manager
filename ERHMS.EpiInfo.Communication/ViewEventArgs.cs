using System;

namespace ERHMS.EpiInfo.Communication
{
    public class ViewEventArgs : EventArgs
    {
        public string ProjectPath { get; private set; }
        public string ViewName { get; private set; }

        public ViewEventArgs(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }
    }
}
