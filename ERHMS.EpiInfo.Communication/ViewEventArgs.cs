using System.Text;

namespace ERHMS.EpiInfo.Communication
{
    public class ViewEventArgs : ProjectEventArgs
    {
        public string ViewName { get; private set; }
        public string Tag { get; private set; }

        public ViewEventArgs(string projectPath, string viewName, string tag = null)
            : base(projectPath)
        {
            ViewName = viewName;
            Tag = tag;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}, {1}", ProjectPath, ViewName);
            if (Tag != null)
            {
                result.AppendFormat(" ({0})", Tag);
            }
            return result.ToString();
        }
    }
}
