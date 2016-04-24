namespace ERHMS.EpiInfo.Communication
{
    public class RecordEventArgs : ViewEventArgs
    {
        public string GlobalRecordId { get; private set; }

        public RecordEventArgs(string projectPath, string viewName, string globalRecordId, string tag = null)
            : base(projectPath, viewName, tag)
        {
            GlobalRecordId = globalRecordId;
        }

        public override string ToString()
        {
            return ToString(ProjectPath, ViewName, GlobalRecordId);
        }
    }
}
