namespace ERHMS.EpiInfo.Communication
{
    public class ProjectEventArgs : EventArgsBase
    {
        public string ProjectPath { get; private set; }

        public ProjectEventArgs(string projectPath, string tag = null)
            : base(tag)
        {
            ProjectPath = projectPath;
        }

        public override string ToString()
        {
            return ToString(ProjectPath);
        }
    }
}
