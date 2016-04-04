namespace ERHMS.EpiInfo.Communication
{
    public class ViewEventArgs : ProjectEventArgs
    {
        public string ViewName { get; private set; }

        public ViewEventArgs(string projectPath, string viewName)
            : base(projectPath)
        {
            ViewName = viewName;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", ProjectPath, ViewName);
        }
    }
}
