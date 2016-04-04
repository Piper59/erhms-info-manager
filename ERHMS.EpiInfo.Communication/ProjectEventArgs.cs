using System;

namespace ERHMS.EpiInfo.Communication
{
    public class ProjectEventArgs : EventArgs
    {
        public string ProjectPath { get; private set; }

        public ProjectEventArgs(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public override string ToString()
        {
            return ProjectPath;
        }
    }
}
