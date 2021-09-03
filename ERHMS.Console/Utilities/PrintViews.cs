using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class PrintViews : Utility
    {
        public string ProjectPath { get; }
        public string FilePath { get; }

        public PrintViews(string projectPath, string filePath)
        {
            ProjectPath = projectPath;
            FilePath = filePath;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            foreach (View view in project.Views)
            {
                PrintView.Run(view, string.Format(FilePath, view.Name));
            }
        }
    }
}
