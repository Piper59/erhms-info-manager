using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Analytics;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public abstract class CreateAsset : IUtility
    {
        public string AssetPath { get; }
        public string ProjectPath { get; }
        public string ViewName { get; }

        public CreateAsset(string assetPath, string projectPath, string viewName)
        {
            AssetPath = assetPath;
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        protected abstract Asset GetAsset(View view);

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            Asset asset = GetAsset(view);
            using (Stream stream = File.Open(AssetPath, FileMode.Create, FileAccess.Write))
            {
                asset.Save(stream);
            }
        }
    }
}
