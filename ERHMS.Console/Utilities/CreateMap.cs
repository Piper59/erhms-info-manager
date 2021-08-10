using Epi;
using ERHMS.EpiInfo.Analytics;

namespace ERHMS.Console.Utilities
{
    public class CreateMap : CreateAsset
    {
        public CreateMap(string mapPath, string projectPath, string viewName)
            : base(mapPath, projectPath, viewName) { }

        protected override Asset GetAsset(View view)
        {
            return new Map(view);
        }
    }
}
