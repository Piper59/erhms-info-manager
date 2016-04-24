namespace ERHMS.EpiInfo.Communication
{
    public class CanvasEventArgs : ProjectEventArgs
    {
        public int CanvasId { get; private set; }
        public string CanvasPath { get; private set; }

        public CanvasEventArgs(string projectPath, int canvasId, string canvasPath, string tag = null)
            : base(projectPath, tag)
        {
            CanvasId = canvasId;
            CanvasPath = canvasPath;
        }

        public override string ToString()
        {
            return ToString(ProjectPath, CanvasId.ToString(), CanvasPath);
        }
    }
}
