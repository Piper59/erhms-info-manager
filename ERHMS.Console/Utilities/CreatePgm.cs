using Epi;
using ERHMS.EpiInfo.Analytics;

namespace ERHMS.Console.Utilities
{
    public class CreatePgm : CreateAsset
    {
        public CreatePgm(string pgmPath, string projectPath, string viewName)
            : base(pgmPath, projectPath, viewName) { }

        protected override Asset GetAsset(View view)
        {
            return new Pgm(view);
        }
    }
}
