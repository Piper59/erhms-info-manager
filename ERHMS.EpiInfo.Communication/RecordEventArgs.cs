namespace ERHMS.EpiInfo.Communication
{
    public class RecordEventArgs : ViewEventArgs
    {
        public string GlobalRecordId { get; private set; }

        public RecordEventArgs(string projectPath, string viewName, string globalRecordId)
            : base(projectPath, viewName)
        {
            GlobalRecordId = globalRecordId;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", ProjectPath, ViewName, GlobalRecordId);
        }
    }
}
