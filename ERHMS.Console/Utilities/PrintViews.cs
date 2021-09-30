using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class PrintViews : Utility
    {
        public string ProjectPath { get; }
        public string FilePathFormat { get; }

        public PrintViews(string projectPath, string filePathFormat)
        {
            ProjectPath = projectPath;
            FilePathFormat = filePathFormat;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            foreach (View view in project.Views)
            {
                PrintView.Run(view, string.Format(FilePathFormat, view.Name));
            }
        }
    }
}
