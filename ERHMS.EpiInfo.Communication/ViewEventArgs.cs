namespace ERHMS.EpiInfo.Communication
{
    public class ViewEventArgs : ProjectEventArgs
    {
        public string ViewName { get; private set; }

        public ViewEventArgs(string projectPath, string viewName, string tag = null)
            : base(projectPath, tag)
        {
            ViewName = viewName;
        }

        public override string ToString()
        {
            return ToString(ProjectPath, ViewName);
        }
    }
}
