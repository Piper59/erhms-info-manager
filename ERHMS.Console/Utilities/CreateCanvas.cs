using Epi;
using ERHMS.EpiInfo.Analytics;

namespace ERHMS.Console.Utilities
{
    public class CreateCanvas : CreateAsset
    {
        public CreateCanvas(string canvasPath, string projectPath, string viewName)
            : base(canvasPath, projectPath, viewName) { }

        protected override Asset GetAsset(View view)
        {
            return new Canvas(view);
        }
    }
}
