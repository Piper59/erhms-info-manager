namespace ERHMS.EpiInfo.Communication
{
    public class PgmEventArgs : ProjectEventArgs
    {
        public int PgmId { get; private set; }

        public PgmEventArgs(string projectPath, int pgmId, string tag = null)
            : base(projectPath, tag)
        {
            PgmId = pgmId;
        }

        public override string ToString()
        {
            return ToString(ProjectPath, PgmId.ToString());
        }
    }
}
